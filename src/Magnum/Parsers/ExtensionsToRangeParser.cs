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
namespace Magnum.Parsers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	public static class ExtensionsToRangeParser
	{
		private static readonly MethodInfo _startsWith;
		private static readonly MethodInfo _compareTo;

		static ExtensionsToRangeParser()
		{
			_startsWith = typeof(string)
				.GetMethods(BindingFlags.Instance | BindingFlags.Public)
				.Where(x => x.Name == "StartsWith")
				.Where(x => x.GetParameters().Count() == 1)
				.Where(x => x.GetParameters().Where(p => p.ParameterType == typeof(string)).Count() == 1)
				.Single();

			_compareTo = typeof(string)
				.GetMethods(BindingFlags.Instance | BindingFlags.Public)
				.Where(x => x.Name == "CompareTo")
				.Where(x => x.GetParameters().Where(p => p.ParameterType == typeof(string)).Count() == 1)
				.Single();
		}

		public static string ToRangeString(this IEnumerable<IRangeElement> elements)
		{
			return string.Join(";", elements.Select(x => x.ToString()).ToArray());
		}

		public static bool Includes(this IEnumerable<IRangeElement> elements, IRangeElement find)
		{
			foreach (IRangeElement element in elements)
			{
				if(element.Includes(find) && !ReferenceEquals(element,find))
					return true;
			}

			return false;
		}

		public static IEnumerable<IRangeElement> Optimize(this IEnumerable<IRangeElement> elements)
		{
			var results = new List<IRangeElement>();

			foreach (IRangeElement element in elements)
			{
				if(results.Contains(element))
					continue;

				if(results.Includes(element))
					continue;

				results.Add(element);
			}

			foreach (IRangeElement result in results)
			{
				if(!results.Includes(result))
					yield return result;
			}
		}

		public static IQueryable<T> WhereInRange<T>(this IQueryable<T> elements, Expression<Func<T,string>> memberExpression, IEnumerable<IRangeElement> rangeElements)
		{
			foreach (IRangeElement rangeElement in rangeElements)
			{
				elements = rangeElement.Where(elements, memberExpression);
			}

			return elements;
		}

		public static IEnumerable<T> WhereInRange<T>(this IEnumerable<T> elements, Expression<Func<T, string>> memberExpression, IEnumerable<IRangeElement> rangeElements)
		{
			foreach (IRangeElement rangeElement in rangeElements)
			{
				elements = rangeElement.Where(elements, memberExpression);
			}

			return elements;
		}

		internal static Expression<Func<T, bool>> ToCompareToExpression<T>(this Expression<Func<T, string>> memberExpression, string value, ExpressionType comparisonType)
		{
			var member = memberExpression.Body as MemberExpression;
			if (member == null)
				throw new InvalidOperationException("Only member expressions are allowed");

			var argument = Expression.Constant(value);
			var zero = Expression.Constant(0);

			var call = Expression.Call(member, _compareTo, new[] { argument });

			var compare = Expression.MakeBinary(comparisonType, call, zero);

			return Expression.Lambda<Func<T, bool>>(compare, new[] { memberExpression.Parameters[0] });
		}

		internal static Expression<Func<T, bool>> ToStartsWithExpression<T>(this Expression<Func<T, string>> memberExpression, string value)
		{
			var member = memberExpression.Body as MemberExpression;
			if (member == null)
				throw new InvalidOperationException("Only member expressions are allowed");

			var argument = Expression.Constant(value);

			var call = Expression.Call(member, _startsWith, new[] { argument });

			return Expression.Lambda<Func<T, bool>>(call, new[] { memberExpression.Parameters[0] });
		}
	}
}