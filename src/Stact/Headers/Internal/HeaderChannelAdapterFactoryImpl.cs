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
namespace Stact.Internal
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.Extensions;


	public class HeaderChannelAdapterFactoryImpl :
		HeaderChannelAdapterFactory
	{
		readonly Type _bodyType;
		readonly Type _headerType;
		readonly bool _matches;
		readonly Type _messageType;
		readonly string _methodName;
		readonly Action<object, HeaderChannel> _router;

		public HeaderChannelAdapterFactoryImpl(Type messageType, Type headerType, string methodName)
		{
			_messageType = messageType;
			_headerType = headerType;
			_methodName = methodName;

			_matches = _messageType.IsGenericType && _messageType.Implements(_headerType);
			if (_matches)
			{
				_bodyType = _messageType.GetGenericTypeDeclarations(_headerType).Single();

				_router = GenerateRouteMethod();
			}
		}

		public bool CanAdapt<TInput>(TInput input, out Action<object, HeaderChannel> adapter)
		{
			adapter = null;

			if (!_matches)
				return false;

			if (!_messageType.Equals(typeof(TInput)))
				return false;

			adapter = _router;
			return true;
		}

		Action<object, HeaderChannel> GenerateRouteMethod()
		{
			ParameterExpression value = Expression.Parameter(typeof(object), "value");
			ParameterExpression output = Expression.Parameter(typeof(HeaderChannel), "output");

			Type messageType = _headerType.MakeGenericType(_bodyType);

			UnaryExpression castValue = Expression.TypeAs(value, messageType);

			MethodInfo method = typeof(HeaderChannel)
				.GetMethod(_methodName)
				.MakeGenericMethod(_bodyType);

			MethodCallExpression call = Expression.Call(output, method, castValue);

			Expression<Action<object, HeaderChannel>> expression = Expression.Lambda<Action<object, HeaderChannel>>(call, value,
			                                                                                                        output);

			return expression.Compile();
		}
	}
}