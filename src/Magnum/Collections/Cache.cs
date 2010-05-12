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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Extensions;

	[Serializable]
	public class Cache<TKey, TValue> :
		IEnumerable<TValue>
	{
		private readonly object _locker = new object();
		private readonly IDictionary<TKey, TValue> _values;
		private Func<TValue, TKey> _keyConverter = DefaultKeyConverter;
		private Action<TKey, TValue> _duplicateValueAddedCallback = DefaultDuplicateValueAddedCallback;
		private Func<TKey, TValue> _missingValueProvider = ThrowOnMissingValue;
		private Action<TKey, TValue> _valueAddedCallback = DefaultValueAddedAction;

		public Cache()
			: this(new Dictionary<TKey, TValue>())
		{
		}

		public Cache(IEqualityComparer<TKey> comparer)
			: this(new Dictionary<TKey, TValue>(comparer))
		{
		}

		public Cache(IDictionary<TKey, TValue> dictionary)
		{
			_values = dictionary;
		}

		public Cache(Func<TKey, TValue> missingValueProvider)
			: this(new Dictionary<TKey, TValue>(), missingValueProvider)
		{
		}

		public Cache(Func<TKey, TValue> missingValueProvider, IEqualityComparer<TKey> comparer)
			: this(new Dictionary<TKey, TValue>(comparer), missingValueProvider)
		{
		}

		public Cache(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> missingValueProvider)
			: this(dictionary)
		{
			_missingValueProvider = missingValueProvider;
		}

		public Func<TKey, TValue> MissingValueProvider
		{
			set { _missingValueProvider = value; }
		}

		public Action<TKey, TValue> ValueAddedCallback
		{
			set { _valueAddedCallback = value; }
		}

		public Func<TValue, TKey> KeyConverter
		{
			get { return _keyConverter; }
			set { _keyConverter = value; }
		}

		public TValue this[TKey key]
		{
			get
			{
				if (!_values.ContainsKey(key))
				{
					lock (_locker)
					{
						if (!_values.ContainsKey(key))
						{
							TValue value = _missingValueProvider(key);
							_values.Add(key, value);
							_valueAddedCallback(key, value);

							return value;
						}
					}
				}

				return _values[key];
			}
			set
			{
				if (_values.ContainsKey(key))
				{
					_values[key] = value;
				}
				else
				{
					_values.Add(key, value);
					_valueAddedCallback(key, value);
				}
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (_values.ContainsKey(key))
			{
				_duplicateValueAddedCallback(key, value);
			}

			this[key] = value;
		}

		public TValue First
		{
			get
			{
				foreach (var pair in _values)
				{
					return pair.Value;
				}

				return default(TValue);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TValue>) this).GetEnumerator();
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return _values.Values.GetEnumerator();
		}

		public void Fill(IEnumerable<TValue> values)
		{
			values.Each(value =>
				{
					TKey key = _keyConverter(value);
					this[key] = value;
				});
		}

		public TValue Retrieve(TKey key)
		{
			TValue value;
			if (_values.TryGetValue(key, out value))
				return value;

			value = _missingValueProvider(key);
			_values.Add(key, value);

			return value;
		}

		public void Each(Action<TValue> action)
		{
			foreach (var pair in _values)
			{
				action(pair.Value);
			}
		}

		public void Each(Action<TKey, TValue> action)
		{
			foreach (var pair in _values)
			{
				action(pair.Key, pair.Value);
			}
		}

		public bool Has(TKey key)
		{
			return _values.ContainsKey(key);
		}

		public bool Exists(Predicate<TValue> predicate)
		{
			bool returnValue = false;

			Each(delegate(TValue value) { returnValue |= predicate(value); });

			return returnValue;
		}

		public TValue Find(Predicate<TValue> predicate)
		{
			foreach (var pair in _values)
			{
				if (predicate(pair.Value))
				{
					return pair.Value;
				}
			}

			return default(TValue);
		}

		public TKey[] GetAllKeys()
		{
			return _values.Keys.ToArray();
		}

		public TValue[] GetAll()
		{
			return _values.Values.ToArray();
		}

		public void Remove(TKey key)
		{
			if (_values.ContainsKey(key))
			{
				_values.Remove(key);
			}
		}

		public void ClearAll()
		{
			_values.Clear();
		}

		public bool WithValue(TKey key, Action<TValue> callback)
		{
			if (Has(key))
			{
				callback(this[key]);
				return true;
			}

			return false;
		}

		private static TValue ThrowOnMissingValue(TKey key)
		{
			throw new KeyNotFoundException("The specified element was not found: " + key);
		}

		private static void DefaultValueAddedAction(TKey key, TValue value)
		{
		}

		private static void DefaultDuplicateValueAddedCallback(TKey key, TValue value)
		{
			throw new InvalidOperationException("Duplicate value added for key: " + key);
		}

		private static TKey DefaultKeyConverter(TValue value)
		{
			throw new InvalidOperationException("No default key converter has been specified");
		}
	}
}