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
namespace Stact.Internal
{
	using System;
	using System.Collections.Generic;
	using TypeConverters;


	public static class HeaderTypeAdapter<T>
	{
		[ThreadStatic]
		static HeaderTypeConverterFactory<T>[] _converterFactories;

		[ThreadStatic]
		static IDictionary<Type, Func<object, T>> _converters;

		[ThreadStatic]
		static HashSet<Type> _ignoredTypes;

		public static bool TryConvert<TInput>(TInput input, Action<T> matchCallback)
		{
			if (_converters == null)
				_converters = new Dictionary<Type, Func<object, T>>();

			Func<object, T> converter;
			if (!_converters.TryGetValue(typeof(TInput), out converter))
			{
				if (_ignoredTypes == null)
					_ignoredTypes = new HashSet<Type>();

				if (_ignoredTypes.Contains(typeof(TInput)))
					return false;

				if (_converterFactories == null)
					_converterFactories = CreateTypeConverters();

				for (int i = 0; i < _converterFactories.Length; i++)
				{
					if (_converterFactories[i].CanConvert(input, out converter))
						break;
				}

				if (converter == null)
				{
					_ignoredTypes.Add(typeof(TInput));
					return false;
				}

				_converters.Add(typeof(TInput), converter);
			}

			if (converter == null)
				return false;

			T output = converter(input);
			matchCallback(output);

			return true;
		}

		static HeaderTypeConverterFactory<T>[] CreateTypeConverters()
		{
			return new HeaderTypeConverterFactory<T>[]
				{
					new MatchingTypeConverterFactory<T>(),
					new AssignableTypeConverterFactory<T>(),
					new MessageTypeConverterFactory<T>(),
					new RequestTypeConverterFactory<T>(),
					new ResponseTypeConverterFactory<T>(),
				};
		}
	}
}