// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.Invoker
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Activator;
	using CollectionExtensions;

	public class FastInvoker<T> :
		FastInvokerBase,
		IFastInvoker<T>
	{
		private static FastInvoker<T> _current;
		private readonly Dictionary<int, Action<T>> _noArgs = new Dictionary<int, Action<T>>();
		private readonly Dictionary<int, Action<T, object[]>> _withArgs = new Dictionary<int, Action<T, object[]>>();

		private FastInvoker()
			: base(typeof (T))
		{
		}

		public static FastInvoker<T> Current
		{
			get
			{
				if (_current == null)
					_current = new FastInvoker<T>();

				return _current;
			}
		}

		public void FastInvoke(T target, string methodName)
		{
			MethodInfo method = MethodNameCache[methodName]
				.MatchingArguments()
				.First();

			GetInvoker(method)(target);
		}

		public void FastInvoke(T target, string methodName, params object[] args)
		{
			if (args == null)
			{
				FastInvoke(target, methodName);
				return;
			}

			MethodInfo method = MethodNameCache[methodName]
				.MatchingArguments(args)
				.First()
				.ToSpecializedMethod(args);

			GetInvoker(method, args)(target, args);
		}

		public void FastInvoke(T target, string methodName, Type[] genericTypes)
		{
			MethodInfo method = MethodNameCache[methodName]
				.MatchingArguments()
				.First()
				.GetGenericMethodDefinition()
				.MakeGenericMethod(genericTypes);

			GetInvoker(method)(target);
		}

		public void FastInvoke(T target, string methodName, Type[] genericTypes, object[] args)
		{
			if (args == null)
			{
				FastInvoke(target, methodName);
				return;
			}

			MethodInfo method = MethodNameCache[methodName]
				.MatchingArguments(args)
				.First()
				.GetGenericMethodDefinition()
				.MakeGenericMethod(genericTypes);

			GetInvoker(method, args)(target, args);
		}

		public void FastInvoke(T target, Expression<Action<T>> expression)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			GetInvoker(call.Method)(target);
		}

		public void FastInvoke(T target, Expression<Action<T>> expression, params object[] args)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			MethodInfo method = call.Method;

			if (method.IsGenericMethod)
				method = method.GetGenericMethodDefinition();

			GetInvoker(method, args)(target, args);
		}

		public void FastInvoke(T target, Expression<Action<T>> expression, Type[] genericTypes)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			MethodInfo method = GetGenericMethodFromTypes(call.Method, genericTypes);

			GetInvoker(method)(target);
		}

		public void FastInvoke(T target, Expression<Action<T>> expression, Type[] genericTypes, object[] args)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			MethodInfo method = GetGenericMethodFromTypes(call.Method, genericTypes);

			GetInvoker(method, args)(target, args);
		}

		private Action<T> GetInvoker(MethodInfo method)
		{
			int key = method.GetHashCode();

			return _noArgs.Retrieve(key, () =>
				{
					ParameterExpression instanceParameter = Expression.Parameter(typeof (T), "target");

					MethodCallExpression call = Expression.Call(instanceParameter, method);

					return Expression.Lambda<Action<T>>(call, new[] {instanceParameter}).Compile();
				});
		}

		private Action<T, object[]> GetInvoker(MethodInfo method, object[] args)
		{
			int key = GetArgumentHashCode(method, args);

			return _withArgs.Retrieve(key, () =>
				{
					if (method.IsGenericMethodDefinition)
						method = GetGenericMethodFromArguments(method, args);

					ParameterExpression instanceParameter = Expression.Parameter(typeof (T), "target");
					ParameterExpression argsParameter = Expression.Parameter(typeof (object[]), "args");

					Expression[] parameters = method.GetParameters().ToArrayIndexParameters(argsParameter).ToArray();

					MethodCallExpression call = Expression.Call(instanceParameter, method, parameters);

					return Expression.Lambda<Action<T, object[]>>(call, new[] {instanceParameter, argsParameter}).Compile();
				});
		}
	}
}