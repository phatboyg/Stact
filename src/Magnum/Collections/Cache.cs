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
namespace Magnum.Collections
{
	using System;
	using System.Collections.Generic;

	public class Cache<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
		private readonly Func<TKey, TValue> _missingValueProvider = ThrowOnMissingValue;

		public Cache()
		{
		}

		public Cache(Func<TKey, TValue> missingValueProvider)
		{
			_missingValueProvider = missingValueProvider;
		}

		public TValue Retrieve(TKey key)
		{
			TValue value;
			if (_dictionary.TryGetValue(key, out value))
				return value;

			value = _missingValueProvider(key);
			_dictionary.Add(key, value);

			return value;
		}

		private static TValue ThrowOnMissingValue(TKey key)
		{
			throw new ArgumentOutOfRangeException("key", "The specified element was not found");
		}
	}
}