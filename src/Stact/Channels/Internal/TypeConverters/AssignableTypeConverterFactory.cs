// Copyright 2010 Chris Patterson
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
namespace Stact.Internal.TypeConverters
{
	using System;
	using System.Linq.Expressions;


	public class AssignableTypeConverterFactory<T> :
		HeaderTypeConverterFactory<T>
	{
		Func<object, T> _convert;

		public AssignableTypeConverterFactory()
		{
			_convert = GenerateConvertMethod();
		}

		public bool CanConvert<TInput>(TInput input, out Func<object, T> converter)
		{
			converter = null;

			if (typeof(T).IsAssignableFrom(typeof(TInput)))
			{
				converter = _convert;
				return true;
			}

			return false;
		}

		static Func<object, T> GenerateConvertMethod()
		{
			ParameterExpression value = Expression.Parameter(typeof(object), "value");

			UnaryExpression castValue;
			if (typeof(T).IsValueType)
				castValue = Expression.Convert(value, typeof(T));
			else
				castValue = Expression.TypeAs(value, typeof(T));

			Expression<Func<object, T>> expression = Expression.Lambda<Func<object, T>>(castValue, value);

			return expression.Compile();
		}
	}
}