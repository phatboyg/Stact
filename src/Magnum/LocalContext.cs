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
	using System;
	using System.Collections;
	using System.Web;
	using Context;

	/// <summary>
	/// Supports the storage of context information through the life of a thread
	/// </summary>
	public static class LocalContext
	{
		private static readonly ILocalContext _current = new LocalContextStorage();
		private static readonly object _localContextHashtableKey = new object();

		/// <summary>
		/// Gets the current data
		/// </summary>
		/// <value>The data.</value>
		public static ILocalContext Current
		{
			get { return _current; }
		}

		/// <summary>
		/// Gets a value indicating whether running in the web context
		/// </summary>
		/// <value><c>true</c> if [running in web]; otherwise, <c>false</c>.</value>
		public static bool RunningInWeb
		{
			get { return HttpContext.Current != null; }
		}

		private class LocalContextStorage :
			ILocalContext
		{
			[ThreadStatic]
			private static Hashtable _threadLocalStorage;

			private static Hashtable ThreadLocalHashtable
			{
				get
				{
					if (!RunningInWeb)
					{
						return _threadLocalStorage ?? (_threadLocalStorage = new Hashtable());
					}

					Hashtable webHashtable = HttpContext.Current.Items[_localContextHashtableKey] as Hashtable;
					if (webHashtable == null)
					{
						HttpContext.Current.Items[_localContextHashtableKey] = webHashtable = new Hashtable();
					}
					return webHashtable;
				}
			}

			public object this[object key]
			{
				get { return ThreadLocalHashtable[key]; }
				set { ThreadLocalHashtable[key] = value; }
			}

			public TValue Retrieve<TValue>()
			{
				return ThreadLocalHashtable.Retrieve<TValue>();
			}

			public TValue Retrieve<TValue>(object key)
			{
				object existing = ThreadLocalHashtable[key];

				TValue value = (TValue) existing;

				return value;
			}

			public TValue Retrieve<TValue>(object key, Func<TValue> valueProvider)
			{
				object existing = ThreadLocalHashtable[key];

				TValue value;
				if (existing == null)
				{
					value = valueProvider();
					ThreadLocalHashtable[key] = value;
				}
				else
				{
					value = (TValue) existing;
				}

				return value;
			}

			public void Remove(object key)
			{
				if (ThreadLocalHashtable.ContainsKey(key))
					ThreadLocalHashtable.Remove(key);
			}

			public bool Contains(object key)
			{
				return ThreadLocalHashtable.ContainsKey(key);
			}

			public void Store<T>(object key, T value)
			{
				ThreadLocalHashtable[key] = value;
			}

			public void Store<T>(T value)
			{
				ThreadLocalHashtable.Store(value);
			}

			public void Clear()
			{
				ThreadLocalHashtable.Clear();
			}
		}
	}
}