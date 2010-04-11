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
namespace Magnum.Extensions
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	public static class ExtensionsToExpression
	{
		/// <summary>
		/// Gets the name of the member specified
		/// </summary>
		/// <typeparam name="T">The type referenced</typeparam>
		/// <typeparam name="TMember">The type of the member referenced</typeparam>
		/// <param name="expression">The expression referencing the member</param>
		/// <returns>The name of the member referenced by the expression</returns>
		public static string MemberName<T, TMember>(this Expression<Func<T, TMember>> expression)
		{
			return expression.GetMemberExpression().Member.Name;
		}

		/// <summary>
		/// Gets the name of the member specified
		/// </summary>
		/// <typeparam name="T">The type referenced</typeparam>
		/// <param name="expression">The expression referencing the member</param>
		/// <returns>The name of the member referenced by the expression</returns>
		public static string MemberName<T>(this Expression<Action<T>> expression)
		{
			return expression.GetMemberExpression().Member.Name;
		}

		/// <summary>
		/// Gets the name of the member specified
		/// </summary>
		/// <typeparam name="T1">The type referenced</typeparam>
		/// <typeparam name="T2">The type of the member referenced</typeparam>
		/// <param name="expression">The expression referencing the member</param>
		/// <returns>The name of the member referenced by the expression</returns>
		public static string MemberName<T1, T2>(this Expression<Action<T1, T2>> expression)
		{
			return expression.GetMemberExpression().Member.Name;
		}

		public static PropertyInfo GetMemberPropertyInfo<T, TMember>(this Expression<Func<T, TMember>> expression)
		{
			return expression.GetMemberExpression().Member as PropertyInfo;
		}

		public static MemberExpression GetMemberExpression<T, TMember>(this Expression<Func<T, TMember>> expression)
		{
			Guard.AgainstNull(expression, "expression");

			return GetMemberExpression(expression.Body);
		}

		public static MemberExpression GetMemberExpression<T>(this Expression<Action<T>> expression)
		{
			Guard.AgainstNull(expression, "expression");

			return GetMemberExpression(expression.Body);
		}

		public static MemberExpression GetMemberExpression<T1, T2>(this Expression<Action<T1, T2>> expression)
		{
			Guard.AgainstNull(expression, "expression");

			return GetMemberExpression(expression.Body);
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

		private static MemberExpression GetMemberExpression(Expression body)
		{
			Guard.AgainstNull(body, "body");

			MemberExpression memberExpression = null;
			if (body.NodeType == ExpressionType.Convert)
			{
				var unaryExpression = (UnaryExpression) body;
				memberExpression = unaryExpression.Operand as MemberExpression;
			}
			else if (body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpression = body as MemberExpression;
			}

			if (memberExpression == null)
				throw new ArgumentException("Expression is not a member access");

			return memberExpression;
		}
	}
}