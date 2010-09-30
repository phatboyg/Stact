// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Stact.Servers.Internal
{
	using System;
	using System.Collections.Specialized;
	using System.IO;
	using System.Net;
	using System.Text;


	public class HttpRequestContext :
		RequestContext
	{
		readonly HttpListenerRequest _request;

		public HttpRequestContext(HttpListenerRequest request)
		{
			_request = request;
		}

		public NameValueCollection Headers
		{
			get { return _request.Headers; }
		}

		public Stream InputStream
		{
			get { return _request.InputStream; }
		}

		public NameValueCollection QueryString
		{
			get { return _request.QueryString; }
		}

		public Uri Url
		{
			get { return _request.Url; }
		}

		public string HttpMethod
		{
			get { return _request.HttpMethod; }
		}

		public string RawUrl
		{
			get { return _request.RawUrl; }
		}

		public Encoding ContentEncoding
		{
			get { return _request.ContentEncoding; }
		}

		public long ContentLength
		{
			get { return _request.ContentLength64; }
		}

		public string ContentType
		{
			get { return _request.ContentType; }
		}

		public CookieCollection Cookies
		{
			get { return _request.Cookies; }
		}

		public string[] AcceptTypes
		{
			get { return _request.AcceptTypes; }
		}

		public bool HasEntityBody
		{
			get { return _request.HasEntityBody; }
		}

		public bool IsAuthenticated
		{
			get { return _request.IsAuthenticated; }
		}

		public bool IsLocal
		{
			get { return _request.IsLocal; }
		}

		public bool IsSecureConnection
		{
			get { return _request.IsSecureConnection; }
		}

		public IPEndPoint LocalEndpoint
		{
			get { return _request.LocalEndPoint; }
		}

		public bool KeepAlive
		{
			get { return _request.KeepAlive; }
		}

		public Version ProtocolVersion
		{
			get { return _request.ProtocolVersion; }
		}

		public object RemoteEndpoint
		{
			get { return _request.RemoteEndPoint; }
		}

		public Uri UrlReferrer
		{
			get { return _request.UrlReferrer; }
		}

		public string UserAgent
		{
			get { return _request.UserAgent; }
		}

		public string UserHostAddress
		{
			get { return _request.UserHostAddress; }
		}

		public string UserHostName
		{
			get { return _request.UserHostName; }
		}

		public string[] UserLanguages
		{
			get { return _request.UserLanguages; }
		}
	}
}