namespace Magnum.Transport.Tests
{
	using System;
	using System.Diagnostics;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading;

	public class PeerInitiator
	{
		private readonly int _connectionTimeout;
		private readonly int _pendingConnectionQueueDepth = 256;
		private readonly int _receiveTimeout;

		public PeerInitiator(TimeSpan connectionTimeout, TimeSpan receiveTimeout)
		{
			_connectionTimeout = connectionTimeout == TimeSpan.MaxValue ? Timeout.Infinite : (int) connectionTimeout.TotalMilliseconds;
			_receiveTimeout = receiveTimeout == TimeSpan.MaxValue ? Timeout.Infinite : (int) connectionTimeout.TotalMilliseconds;
		}

		public void Connect(EndPoint endpoint)
		{
			Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _connectionTimeout);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _receiveTimeout);

			socket.NoDelay = true;
			socket.LingerState = new LingerOption(false, 0);

//			

			InitiatorContext context = new InitiatorContext(socket, endpoint);

			socket.BeginConnect(endpoint, Connect_Callback, context);
		}

		private void Connect_Callback(IAsyncResult ar)
		{
			try
			{
				InitiatorContext context = (InitiatorContext) ar.AsyncState;

				context.Socket.EndConnect(ar);

				PeerConnection connection = new PeerConnection(context.Socket, context.Endpoint, null);

				Connected(connection);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to connect: " + ex.Message);

				ConnectFailed(this);
			}
		}


		public event Action<PeerConnection> Connected = delegate { };
		public event Action<PeerInitiator> ConnectFailed = delegate { };

		public void Listen(IPEndPoint endpoint)
		{
			Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _connectionTimeout);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _receiveTimeout);

			socket.NoDelay = true;
			socket.LingerState = new LingerOption(false, 0);

			InitiatorContext context = new InitiatorContext(socket, endpoint);

			socket.Bind(endpoint);
			socket.Listen(_pendingConnectionQueueDepth);

			PostAccept(context);
		}

		private void PostAccept(InitiatorContext context)
		{
			Socket socket = new Socket(context.Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _connectionTimeout);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _receiveTimeout);

			socket.NoDelay = true;
			socket.LingerState = new LingerOption(false, 0);

			context.Socket.BeginAccept(socket, 0, Accept_Callback, context);
		}

		private void Accept_Callback(IAsyncResult ar)
		{
			InitiatorContext context = (InitiatorContext) ar.AsyncState;

			try
			{
				Socket socket = context.Socket.EndAccept(ar);

				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _connectionTimeout);
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _receiveTimeout);

				socket.NoDelay = true;
				socket.LingerState = new LingerOption(false, 0);

				IPEndPoint remoteIp = socket.RemoteEndPoint as IPEndPoint;

				PeerConnection connection = new PeerConnection(socket, remoteIp, null);

				Connected(connection);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to connect: " + ex.Message);

				ConnectFailed(this);
			}
			finally
			{
				PostAccept(context);
			}
		}

		internal class InitiatorContext
		{
			private readonly EndPoint _endpoint;
			private readonly Socket _socket;

			public InitiatorContext(Socket socket, EndPoint endpoint)
			{
				_endpoint = endpoint;
				_socket = socket;
			}

			public EndPoint Endpoint
			{
				get { return _endpoint; }
			}

			public Socket Socket
			{
				get { return _socket; }
			}
		}
	}
}