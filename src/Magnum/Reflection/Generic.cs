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
namespace Magnum.Reflection
{
	using System;
	using System.Linq.Expressions;
	using Collections;

	public static class Generic
	{
		public static object Call(this object instance, string methodName)
		{
			return _typeCache.Call<object>(instance, methodName, Empty<object>.Array);
		}

		public static object Call(this object instance, string methodName, params object[] args)
		{
			return _typeCache.Call<object>(instance, methodName, args);
		}

		public static object Call(this object instance, string methodName, Type[] argumentTypes, params object[] args)
		{
			return _typeCache.Call<object>(instance, methodName, argumentTypes, args);
		}

		public static T Call<T>(this object instance, string methodName, params object[] args)
		{
			return _typeCache.Call<T>(instance, methodName, args);
		}

		public static T Call<T>(this object instance, string methodName, Type[] argumentTypes, params object[] args)
		{
			return _typeCache.Call<T>(instance, methodName, argumentTypes, args);
		}


		private static readonly MethodCallTypeCache _typeCache = new MethodCallTypeCache();
		private static readonly MethodCache _methodCache = new MethodCache();

		public static void Call(Expression<Action<object>> method, object obj)
		{
			MethodCallExpression methodCall = method.Body as MethodCallExpression;
			if (methodCall != null)
			{
				CallMethod(methodCall, obj);
				return;
			}

			throw new InvalidOperationException("Must be a method call in the expression");
		}

		private static void CallMethod(MethodCallExpression methodCall, object obj)
		{
			if (methodCall.Method.IsGenericMethod)
			{
				_methodCache.Call(methodCall, obj);
				return;
			}

			throw new InvalidOperationException("Support for non-generic methods has not yet been implemented.");
		}
	}
}