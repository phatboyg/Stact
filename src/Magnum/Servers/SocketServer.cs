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
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using Channels;
	using Extensions;
	using Fibers;
	using Logging;


	/// <summary>
	/// SocketServer listens on the specified address and dispatches connections
	/// as they arrive to separate connection objects with their own fibers
	/// </summary>
	public class SocketServer :
		StreamServer<SocketServer>
	{
		const int ConnectionBacklogLimit = 1000;
		const int SendReceiveTimeout = 10000;

		static readonly ILogger _log = Logger.GetLogger<SocketServer>();

		IPEndPoint _endpoint;
		Socket _listener;

		public SocketServer(Uri uri)
			: this(uri, new ThreadPoolFiber(), new ShuntChannel())
		{
		}

		public SocketServer(Uri uri, Fiber fiber)
			: this(uri, fiber, new ShuntChannel())
		{
		}

		public SocketServer(Uri uri, Fiber fiber, UntypedChannel eventChannel)
			: base(uri, fiber, eventChannel)
		{
		}

		protected override void StartListener(Uri uri)
		{
			base.StartListener(uri);

			_endpoint = ResolveEndpoint(uri);

			_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			_listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, SendReceiveTimeout);
			_listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, SendReceiveTimeout);
			_listener.NoDelay = true;
			_listener.LingerState = new LingerOption(false, 0);

			_listener.Bind(_endpoint);
			_listener.Listen(ConnectionBacklogLimit);

			_endpoint = (IPEndPoint)_listener.LocalEndPoint;

			_log.Debug(x => x.Write("LISTEN: {0}", _endpoint));

			QueueAccept();
		}

		static IPEndPoint ResolveEndpoint(Uri uri)
		{
			try
			{
				return uri.ResolveHostName().Single();
			}
			catch (Exception ex)
			{
				throw new StreamServerException(
					"{0} resolves to multiple IP addresses, use 0.0.0.0 to bind to all available addresses".FormatWith(uri), ex);
			}
		}

		void QueueAccept()
		{
			if (CurrentState == Stopping || CurrentState == Stopped)
				return;

			_listener.BeginAccept(AcceptConnection, this);
		}

		void AcceptConnection(IAsyncResult asyncResult)
		{
			try
			{
				Socket socket = _listener.EndAccept(asyncResult);
				if (socket != null)
				{
					DateTime acceptedAt = SystemUtil.UtcNow;
					_log.Debug(x => x.Write("ACCEPT: {0} {1} {2}", socket.Handle.ToInt32(), acceptedAt.ToLongTimeString(), _endpoint));

					ConnectionEstablished(() => HandleConnection(acceptedAt, socket));
				}
			}
			catch (ObjectDisposedException ex)
			{
			}
			catch (Exception ex)
			{
				_log.Warn(x => x.Write(ex, "Failed to accept connection on {0}", _endpoint));
			}
			finally
			{
				QueueAccept();
			}
		}

		void HandleConnection(DateTime acceptedAt, Socket socket)
		{
			int bufferSize = 16384;
			var buffer = new byte[bufferSize];
			SocketError errorCode;
			socket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, out errorCode, asyncResult =>
				{
					try
					{
						int received = socket.EndReceive(asyncResult, out errorCode);
					}
					catch (Exception ex)
					{
						_log.Debug(ex, "SOCKET: Receive Exception");
					}
					finally
					{
						_log.Debug(x => x.Write("CLOSED: {0}", socket.Handle.ToInt32()));
						socket.Close();

						ConnectionComplete();
					}
				}, this);

			// TODO create SocketConnection state machine as well, for connections created from the server
		}

		protected override void ShutdownListener()
		{
			_listener.Close();

			base.ShutdownListener();
		}
	}
}