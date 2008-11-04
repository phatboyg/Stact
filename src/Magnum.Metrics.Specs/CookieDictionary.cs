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
namespace Magnum.Metrics.Specs
{
	using System;
	using System.Collections.Generic;
	using Common.ObjectExtensions;

	public class CookieDictionary : Dictionary<string, string>
	{
		public CookieDictionary(string cookie)
		{
			if (cookie.IsNullOrEmpty())
				return;

			string[] cookies = cookie.Split(new[] {";+"}, 100, StringSplitOptions.RemoveEmptyEntries);

			foreach (string match in cookies)
			{
				string[] item = match.Split('=');
				if (item.Length != 2)
					continue;

				string key = item[0];
				string value = DecodeCookieValue(item[1]);

				Add(key, value);
			}
		}

		private static string DecodeCookieValue(string value)
		{
			value = value.Replace('+', ' ');
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == '%')
				{
					char ch = Convert.ToChar(Convert.ToUInt32(value.Substring(i + 1, 2), 16));
					value = value.Remove(i, 3).Insert(i, ch.ToString());
				}
			}
			return value;
		}
	}
}