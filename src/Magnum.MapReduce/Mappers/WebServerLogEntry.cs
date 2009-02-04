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
namespace Magnum.MapReduce.Mappers
{
	using System;
	using System.Collections.Specialized;
	using System.Web;
	using Magnum.ObjectExtensions;

	public class WebServerLogEntry
	{
		public DateTime Date { get; set; }
		public string SiteName { get; set; }
		public string ComputerName { get; set; }
		public string ServerIpAddress { get; set; }
		public string Method { get; set; }
		public string UriStem { get; set; }
		public string UriQuery { get; set; }
		public int Port { get; set; }
		public string Username { get; set; }
		public string RemoteIpAddress { get; set; }
		public string ProtocolVersion { get; set; }
		public string UserAgent { get; set; }
		public string Cookie { get; set; }
		public string Referer { get; set; }
		public string Host { get; set; }
		public int Status { get; set; }
		public int SubStatus { get; set; }
		public int Win32Status { get; set; }
		public int BytesSent { get; set; }
		public int BytesReceived { get; set; }
		public int TimeTaken { get; set; }

		public NameValueCollection ParseCookies()
		{
			NameValueCollection values = new NameValueCollection();

			if (Cookie.IsNullOrEmpty())
				return values;

			string[] cookies = Cookie.Split(new[] {";+"}, 100, StringSplitOptions.RemoveEmptyEntries);

			foreach (string match in cookies)
			{
				string[] item = match.Split('=');
				if (item.Length != 2)
					continue;

				string key = item[0];
				string value = DecodeCookieValue(item[1]);

				values.Add(key, value);
			}

			return values;
		}

		public NameValueCollection ParseQueryString()
		{
			if (UriQuery.IsNullOrEmpty())
				return new NameValueCollection();

			NameValueCollection values = HttpUtility.ParseQueryString(UriQuery);

			return values;
		}

		private static string DecodeCookieValue(string value)
		{
			value = value.Replace('+', ' ');
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] != '%') continue;

				char ch = Convert.ToChar(Convert.ToUInt32(value.Substring(i + 1, 2), 16));
				value = value.Remove(i, 3).Insert(i, ch.ToString());
			}
			return value;
		}
	}
}