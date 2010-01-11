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
namespace Magnum.RulesEngine.ExecutionModel
{
	using System.Linq.Expressions;
	using System.Reflection;
	using Reflection;

	public class ConditionNormalizer :
		ExpressionVisitor
	{
		public Expression Normalize(Expression expression)
		{
			return Visit(expression);
		}

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			if (m.Method.DeclaringType == typeof (string) && m.Method.Name == "Equals")
			{
				if (m.Method.IsStatic)
				{
					return Expression.Equal(m.Arguments[0], m.Arguments[1]);
				}

				return Expression.Equal(m.Object, m.Arguments[0]);
			}

			return base.VisitMethodCall(m);
		}

		protected override Expression VisitBinary(BinaryExpression b)
		{
			if (b.NodeType == ExpressionType.Equal)
			{
				ConstantExpression c;
				MethodCallExpression m;

				if (((m = b.Left as MethodCallExpression) != null && (c = b.Right as ConstantExpression) != null)
				    || ((m = b.Right as MethodCallExpression) != null && (c = b.Left as ConstantExpression) != null))
				{
					if (c.Value as int? == 0 && m.Method.DeclaringType == typeof (string) && m.Method.Name == "Compare")
					{
						return Expression.Equal(m.Arguments[0], m.Arguments[1]);
					}
				}

				MemberExpression me;
				if (((me = b.Left as MemberExpression) != null && (c = b.Right as ConstantExpression) != null)
				    || ((me = b.Right as MemberExpression) != null && (c = b.Left as ConstantExpression) != null))
				{
					if (me.Member.MemberType == MemberTypes.Property)
					{
						var property = me.Member as PropertyInfo;
						if (property.PropertyType == typeof (bool) && c.Type == typeof (bool))
						{
							var value = c.Value as bool?;

							if (value.Value)
							{
								return me;
							}

							return Expression.Not(me);
						}
					}
				}
			}

			return base.VisitBinary(b);
		}
	}
}