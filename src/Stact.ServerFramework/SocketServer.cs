// Copyright 2010 Chris Patterson
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
namespace Stact.ServerFramework
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using Magnum;
	using Magnum.Extensions;


	/// <summary>
	///   SocketServer listens on the specified address and dispatches connections
	///   as they arrive to separate connection objects with their own fibers
	/// </summary>
	public class SocketServer :
		StreamServer<SocketServer>
	{
		const int ConnectionBacklogLimit = 1000;
		const int SendReceiveTimeout = 10000;

		IPEndPoint _endpoint;
		Socket _listener;

		public SocketServer(Uri uri)
			: this(uri, new PoolFiber(), new ShuntChannel())
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

					ConnectionEstablished(() => HandleConnection(acceptedAt, socket));
				}
			}
			catch (ObjectDisposedException ex)
			{
			}
			catch
			{
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
					catch
					{
					}
					finally
					{
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