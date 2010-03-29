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
namespace Magnum.Web
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	public class DictionaryValueProvider :
		ValueProvider
	{
		private readonly IDictionary<string, object> _values;

		public DictionaryValueProvider(IDictionary<string, object> dictionary)
		{
			_values = dictionary;
		}

		public object GetValue(string key)
		{
			return GetValue(key, null);
		}

		public object GetValue(string key, Func<object> defaultValue)
		{
			object value;
			return _values.TryGetValue(key, out value) ? value : defaultValue();
		}

		public T GetValue<T>(string key)
		{
			object value = GetValue(key);

			if (value == null)
				throw new ArgumentException("The [" + key + "] setting was not found");

			return ConvertValue<T>(key, value);
		}

		public T GetValue<T>(string key, T defaultValue)
		{
			object value;
			return _values.TryGetValue(key, out value) ? ConvertValue<T>(key, value) : defaultValue;
		}

		public T GetValue<T>(string key, Func<object, T> valueConverter)
		{
			object value = GetValue(key);

			if (value == null)
				throw new ArgumentException("The [" + key + "] setting was not found");

			return valueConverter(value);
		}

		public T GetValue<T>(string key, Func<object, T> valueConverter, T defaultValue)
		{
			object value;
			return _values.TryGetValue(key, out value) ? valueConverter(value) : defaultValue;
		}

		private static T ConvertValue<T>(string key, object value)
		{
			TypeConverter tc = TypeDescriptor.GetConverter(typeof (T));

			if (tc.CanConvertFrom(value.GetType()))
			{
				return (T) tc.ConvertFrom(value);
			}

			throw new InvalidOperationException("The [" + key + "] setting could not be converted to type: " + typeof (T).FullName);
		}
	}
}