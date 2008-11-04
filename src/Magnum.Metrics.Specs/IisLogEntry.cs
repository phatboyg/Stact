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

	public class IisLogEntry
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

		public CookieDictionary Cookies
		{
			get { return new CookieDictionary(Cookie); }
		}
	}
}