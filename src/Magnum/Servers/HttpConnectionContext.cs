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
	using Fibers;
	using Logging;


	public class HttpConnectionContext :
		ConnectionContext
	{
		static readonly ILogger _log = Logger.GetLogger<HttpConnectionContext>();

		readonly Fiber _fiber;
		readonly HttpListenerContext _httpContext;
		readonly Action _onConnectionComplete;
		Stopwatch _stopwatch;

		public HttpConnectionContext(Fiber fiber, DateTime acceptedAt, HttpListenerContext httpContext, Action onConnectionComplete)
		{
			_stopwatch = Stopwatch.StartNew();

			_fiber = fiber;
			_httpContext = httpContext;
			_onConnectionComplete = onConnectionComplete;

			_fiber.Add(() =>
				{
					Request = new HttpRequestContext(httpContext.Request);
					ResponseInternal = new HttpResponseContext(httpContext.Response);

					byte[] buffer = new byte[4000];
					int read = Request.InputStream.Read(buffer, 0, buffer.Length);


					FinalizeResonse();

					_stopwatch.Stop();

					_onConnectionComplete();

					_log.Debug(x => x.Write("CLOSED: {0} {1} {2}", httpContext.Request.Url, acceptedAt.ToLongTimeString(), Request.Url));
				});
		}

		protected HttpResponseContext ResponseInternal { get; set; }
		public RequestContext Request { get; set; }

		public ResponseContext Response
		{
			get { return ResponseInternal; }
		}

		public IPrincipal User
		{
			get { return _httpContext.User; }
		}

		public void FinalizeResonse()
		{
			try
			{
				ResponseInternal.OutputStream.Flush();
				ResponseInternal.OutputStream.Dispose();
				_httpContext.Response.Close();
			}
			catch
			{
			}
		}

		public void SetResponseFilter(Func<Stream, Stream> responseFilter)
		{
			ResponseInternal.OutputStream = responseFilter(ResponseInternal.OutputStream);
		}
	}
}