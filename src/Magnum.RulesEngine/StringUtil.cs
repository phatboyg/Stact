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
namespace Magnum.RulesEngine
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using Reflection;

	public static class StringUtil
	{
		public static Expression Normalize(this Expression e)
		{
			MethodCallExpression mce;
			BinaryExpression be;

			if ((mce = e as MethodCallExpression) != null)
			{
				if (mce.Method.DeclaringType == typeof (string) && mce.Method.Name == "Equals")
				{
					if (mce.Method.IsStatic)
					{
						return Expression.Equal(mce.Arguments[0], mce.Arguments[1]);
					}

					return Expression.Equal(mce.Object, mce.Arguments[0]);
				}
			}
			else if ((be = e as BinaryExpression) != null && be.NodeType == ExpressionType.Equal)
			{
				ConstantExpression ce;
				if (((mce = be.Left as MethodCallExpression) != null && (ce = be.Right as ConstantExpression) != null)
				    || ((mce = be.Right as MethodCallExpression) != null && (ce = be.Left as ConstantExpression) != null))
				{
					if (ce.Value as int? == 0 && mce.Method.DeclaringType == typeof (string) && mce.Method.Name == "Compare")
					{
						return Expression.Equal(mce.Arguments[0], mce.Arguments[1]);
					}
				}
			}

			return e;
		}
	}

	public class FlattenExpression :
		ExpressionVisitor
	{
		public List<object> Items { get; private set; }

		public FlattenExpression Flatten(Expression e)
		{
			Items = new List<object>();

			Visit(e);

			return this;
		}

		protected override Expression Visit(Expression exp)
		{
			if(exp == null)
				return null;

			Trace.WriteLine(exp);
			Items.Add(exp);

			return base.Visit(exp);
		}
	}
}