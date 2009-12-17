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

	public class FastInvoker<T, TResult> :
		FastInvokerBase,
		IFastInvoker<T, TResult>
	{
		private static FastInvoker<T, TResult> _current;
		private readonly Dictionary<int, Func<T, TResult>> _noArgs = new Dictionary<int, Func<T, TResult>>();
		private readonly Dictionary<int, Func<T, object[], TResult>> _withArgs = new Dictionary<int, Func<T, object[], TResult>>();

		private FastInvoker()
			: base(typeof (T))
		{
		}

		public static FastInvoker<T, TResult> Current
		{
			get
			{
				if (_current == null)
					_current = new FastInvoker<T, TResult>();

				return _current;
			}
		}

		public TResult FastInvoke(T target, string methodName)
		{
			MethodInfo method = MethodNameCache[methodName]
				.Where(x => x.ReturnType == typeof (TResult))
				.MatchingArguments()
				.First();

			return GetInvoker(method)(target);
		}

		public TResult FastInvoke(T target, string methodName, params object[] args)
		{
			if (args == null)
				return FastInvoke(target, methodName);

			MethodInfo method = MethodNameCache[methodName]
				.Where(x => x.ReturnType == typeof (TResult))
				.MatchingArguments(args)
				.First()
				.ToSpecializedMethod(args);

			return GetInvoker(method, args)(target, args);
		}

		public TResult FastInvoke(T target, string methodName, Type[] genericTypes)
		{
			MethodInfo method = MethodNameCache[methodName]
				.Where(x => x.ReturnType == typeof (TResult))
				.MatchingArguments()
				.First()
				.GetGenericMethodDefinition()
				.MakeGenericMethod(genericTypes);

			return GetInvoker(method)(target);
		}

		public TResult FastInvoke(T target, string methodName, Type[] genericTypes, object[] args)
		{
			if (args == null)
				return FastInvoke(target, methodName);

			MethodInfo method = MethodNameCache[methodName]
				.Where(x => x.ReturnType == typeof (TResult))
				.MatchingArguments(args)
				.First()
				.GetGenericMethodDefinition()
				.MakeGenericMethod(genericTypes);

			return GetInvoker(method, args)(target, args);
		}

		public TResult FastInvoke(T target, Expression<Func<T, TResult>> expression)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			return GetInvoker(call.Method)(target);
		}

		public TResult FastInvoke(T target, Expression<Func<T, TResult>> expression, params object[] args)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			MethodInfo method = call.Method;

			if (method.IsGenericMethod)
				method = method.GetGenericMethodDefinition();

			return GetInvoker(method, args)(target, args);
		}

		public TResult FastInvoke(T target, Expression<Func<T, TResult>> expression, Type[] genericTypes)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			MethodInfo method = GetGenericMethodFromTypes(call.Method, genericTypes);

			return GetInvoker(method)(target);
		}

		public TResult FastInvoke(T target, Expression<Func<T, TResult>> expression, Type[] genericTypes, object[] args)
		{
			var call = expression.Body as MethodCallExpression;
			if (call == null)
				throw new ArgumentException("Only method call expressions are supported.", "expression");

			MethodInfo method = GetGenericMethodFromTypes(call.Method, genericTypes);

			return GetInvoker(method, args)(target, args);
		}

		private Func<T, TResult> GetInvoker(MethodInfo method)
		{
			int key = method.GetHashCode();

			return _noArgs.Retrieve(key, () =>
				{
					ParameterExpression instanceParameter = Expression.Parameter(typeof (T), "target");

					MethodCallExpression call = Expression.Call(instanceParameter, method);

					return Expression.Lambda<Func<T, TResult>>(call, new[] {instanceParameter}).Compile();
				});
		}

		private Func<T, object[], TResult> GetInvoker(MethodInfo method, object[] args)
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

					return Expression.Lambda<Func<T, object[], TResult>>(call, new[] {instanceParameter, argsParameter}).Compile();
				});
		}
	}
}