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
namespace Stact.ServerFramework.Internal
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Security.Principal;


	public class HttpConnectionContext :
		ConnectionContext
	{
		readonly DateTime _acceptedAt;
		readonly HttpListenerContext _httpContext;
		readonly Action _onComplete;
		readonly RequestContext _request;
		readonly HttpResponseContext _response;
		readonly ServerContext _server;
		readonly Stopwatch _timer;

		public HttpConnectionContext(ServerContext server, HttpListenerContext httpContext, DateTime acceptedAt,
		                             Action onComplete)
		{
			_timer = Stopwatch.StartNew();

			_server = server;
			_httpContext = httpContext;
			_acceptedAt = acceptedAt;
			_onComplete = onComplete;

			_request = new HttpRequestContext(httpContext.Request);
			_response = new HttpResponseContext(httpContext.Response);
		}

		public RequestContext Request
		{
			get { return _request; }
		}

		public ResponseContext Response
		{
			get { return _response; }
		}

		public IPrincipal User
		{
			get { return _httpContext.User; }
		}

		public ServerContext Server
		{
			get { return _server; }
		}

		public void Complete()
		{
			_timer.Stop();

			try
			{
				_response.OutputStream.Flush();
				_response.OutputStream.Dispose();
				_httpContext.Response.Close();
			}
			catch
			{
			}
			finally
			{
				_onComplete();
			}
		}

		public void SetResponseFilter(Func<Stream, Stream> responseFilter)
		{
			_response.OutputStream = responseFilter(_response.OutputStream);
		}
	}
}