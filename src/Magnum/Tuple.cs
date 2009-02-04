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
namespace Magnum
{
	using System.Collections.Generic;

	public struct Tuple<TKey, TValue>
	{
		private readonly TKey _key;
		private readonly TValue _value;

		public Tuple(TKey key, TValue value)
		{
			_key = key;
			_value = value;
		}

		public Tuple(KeyValuePair<TKey, TValue> pair)
		{
			_key = pair.Key;
			_value = pair.Value;
		}

		public TValue Value
		{
			get { return _value; }
		}

		public TKey Key
		{
			get { return _key; }
		}

		public bool Equals(Tuple<TKey, TValue> obj)
		{
			return Equals(obj._key, _key) && Equals(obj._value, _value);
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof (Tuple<TKey, TValue>)) return false;
			return Equals((Tuple<TKey, TValue>) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_key.GetHashCode()*397) ^ _value.GetHashCode();
			}
		}
	}
}