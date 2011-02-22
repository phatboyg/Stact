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
namespace Stact.Remote.ReliableMulticast
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using Magnum;
	using Magnum.Extensions;


	public class ReliableMulticastListener :
		IDisposable
	{
		readonly IList<byte[]> _addresses;
		readonly Channel<ArraySegment<byte>> _output;
		readonly TraceSource _ts;
		bool _disposed;
		readonly Uri _multicastAddress;
		ReliableMulticastSocket _socket;


		public ReliableMulticastListener(Uri multicastAddress, Channel<ArraySegment<byte>> output)
		{
			_ts = new TraceSource("Stact.Remote.ReliableMulticast.ReliableMulticastListener", SourceLevels.All);

			_addresses = new List<byte[]>();
			_socket = new ReliableMulticastSocket();
			_multicastAddress = multicastAddress;
			_output = output;

			ReceiveBufferSize = 160*1024;
			ReceiveMessageSize = 64*1024;
		}

		public int ReceiveBufferSize { get; set; }

		public int ReceiveMessageSize { get; set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start()
		{
			_ts.TraceEvent(TraceEventType.Start, 100, "Starting listener on {0}", _multicastAddress);

			_socket.ReceiveBufferSize = ReceiveBufferSize;

			IPEndPoint multicastEndpoint = _multicastAddress.ResolveHostName().Single();
			_socket.Bind(multicastEndpoint);

			_ts.TraceEvent(TraceEventType.Verbose, 0, "Bind listener to {0} complete", multicastEndpoint);

			AddAdditionalInterfaces();

			_socket.EnableHighSpeed();
			_socket.Listen(3);

			_socket.BeginAccept(AcceptConnection, null);

			_ts.TraceEvent(TraceEventType.Stop, 100, "Listener started on {0}", _multicastAddress);
		}

		void AcceptConnection(IAsyncResult asyncResult)
		{
			Socket connection = null;
			try
			{
				if (_disposed)
					return;

				connection = _socket.EndAccept(asyncResult);

				ReceiveFromConnection(connection);
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception)
			{
				connection.CloseAndDispose();
			}
			finally
			{
				if (!_disposed)
					_socket.BeginAccept(AcceptConnection, null);
			}
		}

		void ReceiveFromConnection(Socket connection)
		{
			DateTime acceptedAt = SystemUtil.UtcNow;

			var buffer = new byte[ReceiveMessageSize];
			SocketError errorCode;

			AsyncCallback receiver = null;
			receiver = ar =>
				{
					bool close = false;

					try
					{
						if (_disposed)
							return;

						int length = connection.EndReceive(ar);
						if (length == 0)
						{
							close = true;
							return;
						}

						_output.Send(new ArraySegment<byte>(buffer, 0, length));
					}
					catch (ObjectDisposedException)
					{
					}
					catch (Exception ex)
					{
						Console.WriteLine("Wow: " + ex);
					}
					finally
					{
						if (_disposed || close)
							connection.CloseAndDispose();
						else
							connection.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, out errorCode, receiver, null);
					}
				};

			connection.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, out errorCode, receiver, null);
		}

		void AddAdditionalInterfaces()
		{
			foreach (var address in _addresses)
				_socket.SetPgmOption(ReliableMulticastSocketOptions.AddReceiveIf, address);
		}

		public void AddAddress(string address)
		{
			IPAddress ip = IPAddress.Parse(address);
			AddAddress(ip);
		}

		public void AddAddress(IPAddress address)
		{
			_addresses.Add(address.GetAddressBytes());
		}


		~ReliableMulticastListener()
		{
			Dispose(false);
		}

		void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			if (disposing)
			{
				_disposed = true;

				_socket = _socket.CloseAndDispose();
			}

			_disposed = true;
		}
	}
}