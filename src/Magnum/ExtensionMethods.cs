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
namespace Magnum
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;

	public static class ExtensionMethods
	{

		/// <summary>
		/// Wraps an object that implements IDisposable in an enumeration to make it safe for use in LINQ expressions
		/// </summary>
		/// <typeparam name="T">The type of the object, which must implement IDisposable</typeparam>
		/// <param name="target">The target to wrap</param>
		/// <returns>An enumeration with a single entry equal to the target</returns>
		public static IEnumerable<T> AutoDispose<T>(this T target)
			where T : IDisposable
		{
			try
			{
				yield return target;
			}
			finally
			{
				target.Dispose();
			}
		}


		/// <summary>
		/// Wraps an action expression with no arguments inside an expression that takes an 
		/// argument of the specified type (the argument is ignored, but the original expression is
		/// invoked)
		/// </summary>
		/// <typeparam name="TArgument">The type of argument to accept in the wrapping expression</typeparam>
		/// <param name="expression">The expression to wrap</param>
		/// <returns></returns>
		public static Expression<Action<TArgument>> WrapActionWithArgument<TArgument>(this Expression<Action> expression)
		{
			ParameterExpression parameter = Expression.Parameter(typeof (TArgument), "x");

			return Expression.Lambda<Action<TArgument>>(Expression.Invoke(expression), parameter);
		}
	}
}