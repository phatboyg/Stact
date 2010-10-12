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
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.Extensions;


	public class MessageTypeConverterFactory<T> :
		HeaderTypeConverterFactory<T>
	{
		readonly Func<object, T> _convert;
		readonly IDictionary<Type, Func<object, T>> _typeConvert;

		public MessageTypeConverterFactory()
		{
			_convert = GenerateConvertMethod();

			_typeConvert = new Dictionary<Type, Func<object, T>>();
		}

		public bool CanConvert<TInput>(TInput input, out Func<object, T> converter)
		{
			converter = null;

			if (typeof(TInput).Implements(typeof(Message<>)))
			{
				Type messageType = typeof(TInput).GetGenericTypeDeclarations(typeof(Message<>)).Single();

				if (typeof(T) == messageType)
				{
					converter = _convert;
					return true;
				}

				if (typeof(T).IsAssignableFrom(messageType))
				{
					if (!_typeConvert.TryGetValue(messageType, out converter))
					{
						converter = GenerateConvertMethod(messageType);
						_typeConvert.Add(messageType, converter);
					}

					return true;
				}
			}

			return false;
		}

		static Func<object, T> GenerateConvertMethod()
		{
			ParameterExpression value = Expression.Parameter(typeof(object), "value");

			UnaryExpression castValue = Expression.TypeAs(value, typeof(Message<T>));

			PropertyInfo propertyInfo = ExtensionsToExpression.GetMemberPropertyInfo<Message<T>, T>(x => x.Body);

			MethodCallExpression getProperty = Expression.Call(castValue, propertyInfo.GetGetMethod(true));

			Expression<Func<object, T>> expression = Expression.Lambda<Func<object, T>>(getProperty, value);

			return expression.Compile();
		}

		static Func<object, T> GenerateConvertMethod(Type messageType)
		{
			ParameterExpression value = Expression.Parameter(typeof(object), "value");

			Type headerMessageType = typeof(Message<>).MakeGenericType(messageType);

			UnaryExpression castValue = Expression.TypeAs(value, headerMessageType);

			PropertyInfo propertyInfo = headerMessageType.GetProperty("Body", BindingFlags.Instance | BindingFlags.Public);

			MethodCallExpression getProperty = Expression.Call(castValue, propertyInfo.GetGetMethod(true));

			// value as T is slightly faster than (T)value, so if it's not a value type, use that
			UnaryExpression outputValue;
			if (typeof(T).IsValueType)
				outputValue = Expression.Convert(getProperty, typeof(T));
			else
				outputValue = Expression.TypeAs(getProperty, typeof(T));

			Expression<Func<object, T>> expression = Expression.Lambda<Func<object, T>>(outputValue, value);

			return expression.Compile();
		}
	}
}