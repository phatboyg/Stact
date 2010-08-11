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
namespace Magnum.Sockets
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading;
	using Channels;
	using Events;
	using Fibers;
	using Logging;
	using StateMachine;


	public class SocketServer :
		StateMachine<SocketServer>
	{
		static readonly ILogger _log = Logger.GetLogger<SocketServer>();

		int _connectionBacklogLimit = 1000;
		int _connectionCount;
		IPEndPoint _endpoint;
		UntypedChannel _eventChannel = new ShuntChannel();
		Fiber _fiber;
		Socket _listener;
		int _sendReceiveTimeout = 10000;


		static SocketServer()
		{
			Define(Definition);
		}

		public SocketServer()
			: this(new ThreadPoolFiber(), new ShuntChannel())
		{
		}

		public SocketServer(UntypedChannel eventChannel)
			: this(new ThreadPoolFiber(), eventChannel)
		{
		}

		public SocketServer(Fiber fiber, UntypedChannel eventChannel)
		{
			_fiber = fiber;
			_eventChannel = eventChannel;
		}


		public static Event Start { get; set; }
		public static Event Stop { get; set; }

		public static State Initial { get; set; }
		public static State Starting { get; set; }
		public static State Running { get; set; }
		public static State Stopped { get; set; }
		public static State Stopping { get; set; }

		public static State Completed { get; set; }


		public IPEndPoint Endpoint
		{
			get { return _endpoint; }
		}

		public int ConnectionCount
		{
			get { return _connectionCount; }
		}

		static void Definition()
		{
			Initially(
			          When(Start)
			          	.Call(instance => instance.StartListener())
			          	.TransitionTo(Running)
				);

			During(Running,
			       When(Stop)
			       	.TransitionTo(Stopping));

			Anytime(
			        When(Starting.Enter)
			        	.Then(instance => instance.Publish<SocketServerStarting>()),
			        When(Running.Enter)
			        	.Call(instance => instance.Publish<SocketServerRunning>()),
			        When(Stopping.Enter)
			        	.Call(instance => instance.Publish<SocketServerStopping>())
			        	.Call(instance => instance.StopListener()),
			        When(Stopped.Enter)
			        	.Call(instance => instance.Publish<SocketServerStopped>())
				);
		}

		void StartListener()
		{
			StartListener(new IPEndPoint(IPAddress.Any, 8008));
		}

		void Publish<T>()
			where T : new()
		{
			_eventChannel.Send(new T());
		}

		void StartListener(IPEndPoint endpoint)
		{
			ChangeCurrentState(Starting);

			_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			_listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _sendReceiveTimeout);
			_listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _sendReceiveTimeout);
			_listener.NoDelay = true;
			_listener.LingerState = new LingerOption(false, 0);

			_listener.Bind(endpoint);
			_listener.Listen(_connectionBacklogLimit);

			_endpoint = (IPEndPoint)_listener.LocalEndPoint;

			_log.Debug(x => x.Write("LISTEN: {0}", Endpoint));

			QueueAccept();
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

					int count = Interlocked.Increment(ref _connectionCount);

					_log.Debug(
					           x =>
					           x.Write("ACCEPT: {0} {1} {2} {3}", socket.Handle.ToInt32(), count, acceptedAt.ToLongTimeString(),
					                   Endpoint));

					_fiber.Add(() => HandleConnection(acceptedAt, socket));
				}
			}
			catch (ObjectDisposedException ex)
			{
			}
			catch (Exception ex)
			{
				_log.Warn(x => x.Write(ex, "Failed to accept connection on {0}", Endpoint));
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

		void ConnectionComplete()
		{
			int count = Interlocked.Decrement(ref _connectionCount);

			if (CurrentState == Stopping)
			{
				if (count == 0)
					ShutdownListener();
			}
		}

		void StopListener()
		{
			if (_connectionCount == 0)
				ShutdownListener();
		}

		void ShutdownListener()
		{
			_listener.Close();

			ChangeCurrentState(Stopped);
		}
	}


	public static class ExtensionsToSocketServer
	{
		public static void Start(this SocketServer server)
		{
			server.RaiseEvent(SocketServer.Start);
		}

		public static void Stop(this SocketServer server)
		{
			server.RaiseEvent(SocketServer.Stop);
		}
	}
}