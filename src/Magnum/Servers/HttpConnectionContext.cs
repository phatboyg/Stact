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
namespace Magnum.Servers
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Security.Principal;
	using Logging;


	public class HttpConnectionContext :
		ConnectionContext
	{
		static readonly ILogger _log = Logger.GetLogger<HttpConnectionContext>();

		readonly DateTime _acceptedAt;
		readonly HttpListenerContext _httpContext;
		readonly Action _onComplete;
		readonly Stopwatch _stopwatch;

		HttpResponseContext _response;

		public HttpConnectionContext(DateTime acceptedAt, HttpListenerContext httpContext, Action onComplete)
		{
			_stopwatch = Stopwatch.StartNew();

			_acceptedAt = acceptedAt;
			_httpContext = httpContext;
			_onComplete = onComplete;

			Request = new HttpRequestContext(httpContext.Request);
			_response = new HttpResponseContext(httpContext.Response);
		}

		public RequestContext Request { get; private set; }

		public ResponseContext Response
		{
			get { return _response; }
		}

		public IPrincipal User
		{
			get { return _httpContext.User; }
		}

		public void Complete()
		{
			_stopwatch.Stop();

			try
			{
				_response.OutputStream.Flush();
				_response.OutputStream.Dispose();
				_httpContext.Response.Close();

				_log.Debug(x => x.Write("CLOSED: {0} {1} {2}", Request.Url, _acceptedAt.ToLongTimeString(), Request.Url));

				IsCompleted = true;
			}
			catch
			{
			}
			finally
			{
				_onComplete();
			}
		}

		public bool IsCompleted { get; private set; }

		public void SetResponseFilter(Func<Stream, Stream> responseFilter)
		{
			_response.OutputStream = responseFilter(_response.OutputStream);
		}
	}
}