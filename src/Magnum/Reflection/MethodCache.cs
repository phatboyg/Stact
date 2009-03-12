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
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using CollectionExtensions;
	using Collections;
	using ObjectExtensions;

	public class MethodCache
	{
		private static readonly Dictionary<Tuple<Type, Type>, Action<object, object>> _methods;

		static MethodCache()
		{
			_methods = new Dictionary<Tuple<Type, Type>, Action<object, object>>();
		}

		public void Call(MethodCallExpression methodCall, object arg)
		{
			if (arg == null)
				throw new ArgumentNullException("arg", "Argument must not be null");

			ConstantExpression instanceObject = methodCall.Object as ConstantExpression;
			if (instanceObject == null)
				throw new ArgumentNullException("methodCall", "Method call cannot be on a null object");

			Type instanceType = instanceObject.Value.GetType();
			Type argType = arg.GetType();

			var invoker = _methods.Retrieve(new Tuple<Type, Type>(instanceType, argType), () =>
				{
					MethodInfo genericMethodDefinition = methodCall.Method.GetGenericMethodDefinition();
					genericMethodDefinition.MustNotBeNull("genericMethodDefinition");

					MethodInfo methodInfo = genericMethodDefinition.MakeGenericMethod(argType);

					var instance = Expression.Parameter(typeof (object), "instance");
					var inputArg = Expression.Parameter(typeof (object), "arg");
					ParameterExpression[] arguments = {instance, inputArg};

					var instanceCast = Expression.TypeAs(instance, instanceType);
					var valueCast = argType.IsValueType ? Expression.Convert(inputArg, argType) : Expression.TypeAs(inputArg, argType);

					var call = Expression.Call(instanceCast, methodInfo, valueCast);

					return Expression.Lambda<Action<object, object>>(call, arguments).Compile();
				});

			invoker(instanceObject.Value, arg);
		}
	}
}