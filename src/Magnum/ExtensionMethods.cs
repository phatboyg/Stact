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
	using System.Collections.Generic;
	using System.Linq.Expressions;

	public static class ExtensionMethods
	{
        /// <summary>
        /// Enumerates a collection, calling the specified action for each entry in the collection
        /// </summary>
        /// <typeparam name="T">The type of the enumeration</typeparam>
        /// <param name="collection">The collection to enumerate</param>
        /// <param name="callback">The action to call for each entry in the collection</param>
        /// <returns>The collection that was enumerated</returns>
		public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> callback)
		{
			foreach (T item in collection)
			{
				callback(item);
			}

			return collection;
		}

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
		/// Gets the name of the member specified
		/// </summary>
		/// <typeparam name="T">The type referenced</typeparam>
		/// <typeparam name="V">The type of the member referenced</typeparam>
		/// <param name="expression">The expression referencing the member</param>
		/// <returns>The name of the member referenced by the expression</returns>
		public static string MemberName<T, V>(this Expression<Func<T, V>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
				throw new InvalidOperationException("Expression must be a member expression");

			return memberExpression.Member.Name;
		}

		/// <summary>
		/// Gets the name of the member specified
		/// </summary>
		/// <typeparam name="T">The type referenced</typeparam>
		/// <typeparam name="V">The type of the member referenced</typeparam>
		/// <param name="expression">The expression referencing the member</param>
		/// <returns>The name of the member referenced by the expression</returns>
		public static string MemberName<T>(this Expression<Action<T>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
				throw new InvalidOperationException("Expression must be a member expression");

			return memberExpression.Member.Name;
		}
		public static string MemberName<T1,T2>(this Expression<Action<T1,T2>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
				throw new InvalidOperationException("Expression must be a member expression");

			return memberExpression.Member.Name;
		}
	}
}