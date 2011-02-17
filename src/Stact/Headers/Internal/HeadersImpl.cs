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
	using System.Collections;
	using System.Collections.Generic;


	public class HeadersImpl :
		Headers
	{
		readonly IDictionary<string, string> _headers;

		public HeadersImpl(IDictionary<string, string> headers)
		{
			_headers = headers;
		}

		public HeadersImpl()
			: this(new Dictionary<string, string>())
		{
		}

		public string this[string key]
		{
			get
			{
				string value;
				_headers.TryGetValue(key, out value);
				return value;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
					_headers.Remove(key);
				else
					_headers[key] = value;
			}
		}

		public IDictionary<string, string> GetDictionary()
		{
			return _headers;
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return _headers.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}