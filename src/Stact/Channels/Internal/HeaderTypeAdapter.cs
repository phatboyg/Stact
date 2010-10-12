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


	public class HeaderTypeAdapter<T>
	{
		readonly ThreadSingleton<T, HeaderTypeConverter<T>[]> _conversions;
		readonly ThreadSingleton<HeaderTypeAdapter<T>, IDictionary<Type, Func<object, T>>> _converters;
		readonly ThreadSingleton<HeaderTypeAdapter<T>,HashSet<Type>> _ignored;

		public HeaderTypeAdapter()
		{
			_converters = new ThreadSingleton<HeaderTypeAdapter<T>, IDictionary<Type, Func<object, T>>>(() => new Dictionary<Type, Func<object, T>>());
			_ignored = new ThreadSingleton<HeaderTypeAdapter<T>, HashSet<Type>>(() => new HashSet<Type>());

			Func<HeaderTypeConverter<T>[]> converterFactory = () =>
				{
					return new HeaderTypeConverter<T>[]
						{
							new MatchingTypeConverter<T>(),
							new AssignableTypeConverter<T>(),
							new MessageTypeConverter<T>(),
						};
				};

			_conversions = new ThreadSingleton<T, HeaderTypeConverter<T>[]>(converterFactory);
		}

		public bool TryConvert<TInput>(TInput input, Action<T> matchCallback)
		{
			Func<object, T> converter;
			if (!_converters.Value.TryGetValue(typeof(TInput), out converter))
			{
				if (_ignored.Value.Contains(typeof(TInput)))
					return false;

				var conversions = _conversions.Value;
				for (int i = 0; i < conversions.Length; i++)
				{
					if (conversions[i].CanConvert(input, out converter))
						break;
				}

				if (converter == null)
				{
					_ignored.Value.Add(typeof(TInput));
					return false;
				}

				_converters.Value.Add(typeof(TInput), converter);
			}

			if (converter == null)
				return false;

			T output = converter(input);
			matchCallback(output);

			return true;
		}
	}
}