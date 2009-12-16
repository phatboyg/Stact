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
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Activator;

	public static class FastInvokerFactory
	{
		public static Func<T, object[], object> Create<T>(MethodInfo method)
		{
			ParameterExpression instanceParameter = Expression.Parameter(typeof (T), "target");
			ParameterExpression argumentsParameter = Expression.Parameter(typeof (object[]), "args");

			Expression[] parameters = method.GetParameters().ToArgumentsExpression(argumentsParameter).ToArray();

			MethodCallExpression call = Expression.Call(
				instanceParameter,
				method,
				parameters);

			Expression<Func<T, object[], object>> lambda = Expression.Lambda<Func<T, object[], object>>(
				Expression.Convert(call, typeof (object)),
				new[] {instanceParameter, argumentsParameter});

			return lambda.Compile();
		}
	}
}