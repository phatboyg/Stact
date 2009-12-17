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
		private readonly Dictionary<int, Action<T, object[]>> _actionArgs = new Dictionary<int, Action<T, object[]>>();
		private readonly Dictionary<int, Action<T>> _actionT = new Dictionary<int, Action<T>>();

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

		public void FastInvoke(T target, string methodName, params object[] args)
		{
            if(args == null)
                args = new object[]{};

			MethodInfo method = typeof (T).GetMethods()
				.Where(x => x.Name == methodName)
				.MatchingArguments(args)
				.First();

			InvokeWithArguments(target, method, args);
		}

		public void FastInvoke(T target, Expression<Action<T>> expression, params object[] args)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			InvokeWithArguments(target, call.Method, args);
		}

		public void FastInvoke(T target, Expression<Action<T>> expression)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			InvokeWithNoArguments(target, call.Method);
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

		private void InvokeWithArguments(T target, MethodInfo method, object[] args)
		{
			int offset = 0;
			int key = method.GetHashCode() ^ args.Aggregate(0, (x, o) => x ^ (o == null ? offset : o.GetType().GetHashCode() << offset++));

			Action<T, object[]> invoker = _actionArgs.Retrieve(key, () =>
				{
                    if (method.IsGenericMethod)
                    {
                        method = new[] {method.GetGenericMethodDefinition()}
                            .MatchingArguments(args)
                            .First()
                            .ToSpecializedMethod(args);
                    }

				    ParameterExpression instanceParameter = Expression.Parameter(typeof (T), "target");
					ParameterExpression argsParameter = Expression.Parameter(typeof (object[]), "args");

					Expression[] parameters = method.GetParameters().ToArgumentsExpression(argsParameter).ToArray();

					MethodCallExpression call = Expression.Call(instanceParameter, method, parameters);

					Expression<Action<T, object[]>> lambda = Expression.Lambda<Action<T, object[]>>(call, new[] {instanceParameter, argsParameter});

					return lambda.Compile();
				});

			invoker(target, args);
		}

		public static void Invoke(T target, Expression<Action<T>> expression)
		{
			Current.FastInvoke(target, expression);
		}

		public static void Invoke(T target, Expression<Action<T>> expression, params object[] args)
		{
			Current.FastInvoke(target, expression, args);
		}

		public static void Invoke(T target, string methodName, params object[] args)
		{
			Current.FastInvoke(target, methodName, args);
		}
	}
}