// Copyright 2010 Chris Patterson
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Stact.MessageHeaders
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.Extensions;


	public class BodySelectorFactory :
		MatchHeaderSelectorFactory
	{
		readonly bool _includeInherited;
		readonly Type _messageType;
		bool _matches;
		string _methodName;
		Action<object, MatchHeaderCallback> _router;

		public BodySelectorFactory(Type messageType, bool includeInherited = false)
		{
			_messageType = messageType;
			_includeInherited = includeInherited;

			_methodName = "Message";

			_matches = !_messageType.IsGenericType || !_messageType.Implements(typeof(Message<>));
			if (_matches)
				_router = GenerateRouteMethod(_messageType);
		}

		public bool CanMatch<TInput>(TInput input, out Action<object, MatchHeaderCallback> adapter)
		{
			adapter = null;

			if (!_matches)
				return false;

			if (!_messageType.Equals(typeof(TInput)))
				return false;

			if (_includeInherited)
				adapter = GenerateAdapterForInheritedTypes(_messageType);
			else
				adapter = _router;
			return true;
		}

		public bool CanMatch<TContext, TInput>(TInput input,
		                                       out Action<TContext, object, MatchHeaderCallback<TContext>> adapter)
		{
			adapter = null;

			if (!_matches)
				return false;

			if (!_messageType.Equals(typeof(TInput)))
				return false;

			if (_includeInherited)
				adapter = GenerateContextAdapterForInheritedTypes<TContext>(_messageType);
			else
				adapter = GenerateContextMethod<TContext>(_messageType);
			return true;
		}

		IEnumerable<Type> GetInheritedTypes(Type type)
		{
			foreach (Type interfaceType in type.GetInterfaces())
				yield return interfaceType;

			Type baseType = type.BaseType;
			while (baseType != null && baseType != typeof(object))
			{
				yield return baseType;
				baseType = baseType.BaseType;
			}
		}

		Action<object, MatchHeaderCallback> GenerateAdapterForInheritedTypes(Type bodyType)
		{
			Action<object, MatchHeaderCallback>[] adapters = Enumerable.Repeat(_router, 1)
				.Union(GetInheritedTypes(bodyType).Select(GenerateRouteMethod))
				.ToArray();

			return (message, callback) => {
				for (int i = 0; i < adapters.Length; i++)
					adapters[i](message, callback);
			};
		}

		Action<TContext, object, MatchHeaderCallback<TContext>> GenerateContextAdapterForInheritedTypes<TContext>(Type bodyType)
		{
			Action<TContext, object, MatchHeaderCallback<TContext>>[] adapters =
				Enumerable.Repeat(GenerateContextMethod<TContext>(bodyType), 1)
					.Union(GetInheritedTypes(bodyType).Select(GenerateContextMethod<TContext>))
					.ToArray();

			return (context, message, callback) => {
				for (int i = 0; i < adapters.Length; i++)
					adapters[i](context, message, callback);
			};
		}


		Action<object, MatchHeaderCallback> GenerateRouteMethod(Type bodyType)
		{
			ParameterExpression value = Expression.Parameter(typeof(object), "value");
			ParameterExpression output = Expression.Parameter(typeof(MatchHeaderCallback), "output");

			Type messageType = typeof(Message<>).MakeGenericType(bodyType);

			UnaryExpression castValue;
			if (bodyType.IsValueType)
				castValue = Expression.Convert(value, bodyType);
			else
				castValue = Expression.TypeAs(value, bodyType);

			Type messageImplType = typeof(MessageImpl<>).MakeGenericType(bodyType);

			ConstructorInfo constructorInfo = messageImplType.GetConstructor(new[] {bodyType});

			NewExpression constructor = Expression.New(constructorInfo, castValue);

			MethodInfo method = typeof(MatchHeaderCallback)
				.GetMethod(_methodName)
				.MakeGenericMethod(bodyType);

			UnaryExpression headerCast = Expression.TypeAs(constructor, messageType);

			MethodCallExpression call = Expression.Call(output, method, headerCast);

			Expression<Action<object, MatchHeaderCallback>> expression =
				Expression.Lambda<Action<object, MatchHeaderCallback>>(call, value, output);

			return expression.Compile();
		}

		Action<TContext, object, MatchHeaderCallback<TContext>> GenerateContextMethod<TContext>(Type bodyType)
		{
			ParameterExpression context = Expression.Parameter(typeof(TContext), "context");
			ParameterExpression value = Expression.Parameter(typeof(object), "value");
			ParameterExpression output = Expression.Parameter(typeof(MatchHeaderCallback<TContext>), "output");

			Type messageType = typeof(Message<>).MakeGenericType(bodyType);

			UnaryExpression castValue;
			if (bodyType.IsValueType)
				castValue = Expression.Convert(value, bodyType);
			else
				castValue = Expression.TypeAs(value, bodyType);

			Type messageImplType = typeof(MessageImpl<>).MakeGenericType(bodyType);

			ConstructorInfo constructorInfo = messageImplType.GetConstructor(new[] { bodyType });

			NewExpression constructor = Expression.New(constructorInfo, castValue);

			MethodInfo method = typeof(MatchHeaderCallback<TContext>)
				.GetMethod(_methodName)
				.MakeGenericMethod(bodyType);

			// need to downgrade context to mapped type !!!


			UnaryExpression headerCast = Expression.TypeAs(constructor, messageType);

			MethodCallExpression call = Expression.Call(output, method, context, headerCast);

			Expression<Action<TContext, object, MatchHeaderCallback<TContext>>> expression =
				Expression.Lambda<Action<TContext, object, MatchHeaderCallback<TContext>>>(call, context, value, output);

			return expression.Compile();
		}
	}
}