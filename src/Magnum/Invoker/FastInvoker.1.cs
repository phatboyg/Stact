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
		IFastInvoker<T>
	{
		private static FastInvoker<T> _current;
		private readonly Dictionary<int, Action<T>> _actionT = new Dictionary<int, Action<T>>();
		private readonly Dictionary<int, Action<T, object>> _actionT1 = new Dictionary<int, Action<T, object>>();

		private FastInvoker()
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

		public void FastInvoke(T target, Expression<Action<T>> expression)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			InvokeWithNoArguments(target, call.Method);
		}

		public void FastInvoke(T target, Expression<Action<T>> expression, object arg0)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			InvokeWithOneArgument(target, call.Method, arg0);
		}


		private void InvokeWithNoArguments(T target, MethodInfo method)
		{
			int key = method.GetHashCode();

			Action<T> invoker = _actionT.Retrieve(key, () =>
				{
					ParameterExpression instanceParameter = Expression.Parameter(typeof (T), "target");

					MethodCallExpression call = Expression.Call(instanceParameter, method);

					Expression<Action<T>> lambda = Expression.Lambda<Action<T>>(call, new[] {instanceParameter});

					return lambda.Compile();
				});

			invoker(target);
		}

		private void InvokeWithOneArgument(T target, MethodInfo method, object arg0)
		{
			int key = method.GetHashCode() ^ (arg0 == null ? 0 : arg0.GetType().GetHashCode());

			Action<T, object> invoker = _actionT1.Retrieve(key, () =>
				{
					var args = new[] {arg0};

					method = new[] {method.GetGenericMethodDefinition()}
						.MatchingArguments(arg0)
						.First()
						.ToSpecializedMethod(args);

					ParameterExpression instanceParameter = Expression.Parameter(typeof (T), "target");
					ParameterExpression arg0Parameter = Expression.Parameter(typeof (object), "arg0");

					Type arg0Type = method.GetParameters().First().ParameterType;
					UnaryExpression convertArg0 = Expression.Convert(arg0Parameter, arg0Type);

					MethodCallExpression call = Expression.Call(instanceParameter, method, convertArg0);

					Expression<Action<T, object>> lambda = Expression.Lambda<Action<T, object>>(call, new[] {instanceParameter, arg0Parameter});

					return lambda.Compile();
				});

			invoker(target, arg0);
		}

		public static void Invoke(T target, Expression<Action<T>> expression)
		{
			Current.FastInvoke(target, expression);
		}

		public static void Invoke(T target, Expression<Action<T>> expression, object arg0)
		{
			Current.FastInvoke(target, expression, arg0);
		}
	}
}