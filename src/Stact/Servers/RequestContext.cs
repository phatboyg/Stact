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
namespace Stact.Servers
{
	using System;
	using System.Collections.Specialized;
	using System.IO;
	using System.Net;
	using System.Text;


	/// <summary>
	/// Wraps a request, including the Url, Headers, and any QueryString parameters
	/// </summary>
	public interface RequestContext
	{
		/// <summary>
		/// The headers submitted as part of the request
		/// </summary>
		NameValueCollection Headers { get; }

		/// <summary>
		/// The input stream to read the request contents
		/// </summary>
		Stream InputStream { get; }

		/// <summary>
		/// The query string arguments that were included with the request
		/// </summary>
		NameValueCollection QueryString { get; }

		/// <summary>
		/// The method (GET,PUT,etc.)
		/// </summary>
		string HttpMethod { get; }

		/// <summary>
		/// The URL specified for the request
		/// </summary>
		Uri Url { get; }

		/// <summary>
		/// The raw URL specified
		/// </summary>
		string RawUrl { get; }

		Encoding ContentEncoding { get; }

		long ContentLength { get; }
		string ContentType { get; }
		CookieCollection Cookies { get; }
		string[] AcceptTypes { get; }
		bool HasEntityBody { get; }
		bool IsAuthenticated { get; }
		bool IsLocal { get; }
		bool IsSecureConnection { get; }
		IPEndPoint LocalEndpoint { get; }
		bool KeepAlive { get; }
		Version ProtocolVersion { get; }
		object RemoteEndpoint { get; }
		Uri UrlReferrer { get; }
		string UserAgent { get; }
		string UserHostAddress { get; }
		string UserHostName { get; }
		string[] UserLanguages { get; }
	}
}