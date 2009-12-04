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
namespace Magnum.Generator
{
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;

	public static class ExpressionExtensions
	{
		public static ParameterExpression ToParameterExpression(this ParameterInfo parameterInfo)
		{
			return Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name ?? "x");
		}

		public static ParameterExpression ToParameterExpression(this ParameterInfo parameterInfo, string name)
		{
			return Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name ?? name);
		}

		public static IEnumerable<ParameterExpression> ToParameterExpressions(this IEnumerable<ParameterInfo> parameters)
		{
			int index = 0;
			foreach (ParameterInfo parameter in parameters)
			{
				yield return parameter.ToParameterExpression("arg" + index++);
			}
		}

		public static IEnumerable<Expression> ToObjectArrayExpression(this IEnumerable<ParameterInfo> parameters, ParameterExpression objectArrayParameter)
		{
			int index = 0;
			foreach (ParameterInfo parameter in parameters)
			{
				var arrayIndex = Expression.Constant(index++);

				var arrayAccessor = Expression.ArrayIndex(objectArrayParameter, arrayIndex);

				var cast = Expression.Convert(arrayAccessor, parameter.ParameterType);

				yield return cast;
			}
		}
	}
}