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
	using System.IO.Compression;
	using System.Linq;
	using System.Net;
	using Channels;
	using Fibers;
	using Logging;


	public class HttpServer :
		StreamServer<HttpServer>,
		ServerContext
	{
		static readonly ILogger _log = Logger.GetLogger<HttpServer>();

		int _concurrentConnectionLimit = 1000;
		ChannelAdapter<ConnectionContext> _connectionChannel;
		ChannelConnection _connectionChannelConnection;
		HttpListener _httpListener;

		public HttpServer(Uri uri, Fiber fiber, UntypedChannel eventChannel)
			: base(uri, fiber, eventChannel)
		{
		}

		protected override void StartListener(Uri uri)
		{
			base.StartListener(uri);

			string prefix = GetPrefixForUri(uri);

			CreateChannelNetwork();

			_httpListener = new HttpListener();
			_httpListener.Prefixes.Add(prefix);

			// TODO consider mapping the access types/schemes
			_httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

			_log.Debug(x => x.Write("Starting HttpListener on {0}", prefix));
			_httpListener.Start();

			QueueAccept();
		}

		void CreateChannelNetwork()
		{
			_connectionChannel = new ChannelAdapter<ConnectionContext>();
			_connectionChannelConnection = _connectionChannel.Connect(x =>
				{
					var channelProvider = new HttpConnectionChannelProvider();
					var threadPoolChannel = new ThreadPoolChannel<ConnectionContext>(channelProvider, _concurrentConnectionLimit);

					x.AddChannel(threadPoolChannel);
				});
		}

		void QueueAccept()
		{
			if (CurrentState == Stopping || CurrentState == Stopped)
				return;

			_httpListener.BeginGetContext(GetContext, null);
		}

		void GetContext(IAsyncResult asyncResult)
		{
			try
			{
				HttpListenerContext context = _httpListener.EndGetContext(asyncResult);

				DateTime acceptedAt = SystemUtil.UtcNow;
				_log.Debug(
				           x =>
				           x.Write("ACCEPT: {0} {1} {2}", context.Request.Url, acceptedAt.ToLongTimeString(), context.Request.RawUrl));

				ConnectionEstablished(() => HandleConnection(acceptedAt, context));
			}
			catch (InvalidOperationException)
			{
			}
			catch (HttpListenerException)
			{
			}
			finally
			{
				QueueAccept();
			}
		}

		void HandleConnection(DateTime acceptedAt, HttpListenerContext httpContext)
		{
			try
			{
				_connectionChannel.Send(new HttpConnectionContext(acceptedAt, httpContext, ConnectionComplete));
			}
			catch (Exception ex)
			{
				_log.Error(x => x.Write(ex, "FAILED: {0}", httpContext.Request.Url));
			}
		}

		protected override void ShutdownListener()
		{
			_httpListener.Close();
			_connectionChannelConnection.Dispose();

			base.ShutdownListener();
		}

		static string GetPrefixForUri(Uri uri)
		{
			if (false == new[] {"http", "https"}.Contains(uri.Scheme.ToLowerInvariant()))
				throw new ArgumentException("The Uri must be an http/https address: " + uri, "uri");

			string prefix = uri.ToString();

			if (UriHostNameType.IPv4 == uri.HostNameType
			    && IPAddress.Any.GetAddressBytes().SequenceEqual(IPAddress.Parse(uri.Host).GetAddressBytes()))
				prefix = prefix.Replace("://0.0.0.0", "://+");

			if (!prefix.EndsWith("/"))
				prefix += "/";

			return prefix;
		}

		static void AddHttpCompressionIfClientCanAcceptIt(HttpConnectionContext context)
		{
			string acceptEncoding = context.Request.Headers["Accept-Encoding"];

			if (string.IsNullOrEmpty(acceptEncoding))
				return;

			if ((acceptEncoding.IndexOf("gzip", StringComparison.InvariantCultureIgnoreCase) != -1))
			{
				context.SetResponseFilter(s => new GZipStream(s, CompressionMode.Compress, true));
				context.Response.Headers["Content-Encoding"] = "gzip";
			}
			else if (acceptEncoding.IndexOf("deflate", StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				context.SetResponseFilter(s => new DeflateStream(s, CompressionMode.Compress, true));
				context.Response.Headers["Content-Encoding"] = "deflate";
			}
		}
	}
}