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
namespace Magnum.Specs.Sockets
{
	using System.Collections.Generic;
	using System.IO;
	using System.Net.Sockets;
	using System.Threading;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using Magnum.Logging;
	using Magnum.Sockets;
	using Magnum.Sockets.Events;
	using NUnit.Framework;
	using TestFramework;


	[Scenario]
	public class When_starting_a_socket_server
	{
		protected SocketServer _server;
		ChannelAdapter _input;
		Future<SocketServerStarting> _startingEventReceived;
		Future<SocketServerRunning> _runningEventReceived;
		ChannelConnection _connection;

		[When]
		public void Starting_a_socket_server()
		{
			TraceLogProvider.Configure(LogLevel.Info);

			_startingEventReceived = new Future<SocketServerStarting>();
			_runningEventReceived = new Future<SocketServerRunning>();

			_input = new ChannelAdapter();
			_connection = _input.Connect(x =>
				{
					x.AddConsumerOf<SocketServerStarting>()
						.UsingConsumer(_startingEventReceived.Complete)
						.ExecuteOnProducerThread();

					x.AddConsumerOf<SocketServerRunning>()
						.UsingConsumer(_runningEventReceived.Complete)
						.ExecuteOnProducerThread();
				});
			
			_server = new SocketServer(new ThreadPoolFiber(), _input);
			_server.Start();
		}

		[After]
		public void Finally()
		{
			_server.Stop();

			_connection.Dispose();
		}

		[Then]
		public void Should_be_available_to_accept_connections()
		{
			_server.CurrentState.ShouldEqual(SocketServer.Running);
		}

		[Test]
		public void Should_have_called_the_starting_event()
		{
			_startingEventReceived.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();			
		}

		[Test]
		public void Should_have_called_the_running_event()
		{
			_runningEventReceived.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}
	}

	[Scenario]
	public class When_connecting_to_a_socket_server :
		When_starting_a_socket_server
	{
		TcpClient _tcpClient;

		[When]
		public void Connecting_to_a_socket_server()
		{
			_tcpClient = new TcpClient();
			_tcpClient.Connect("localhost", 8008);
		}

		[After]
		public void My_Finally()
		{
			_tcpClient.Close();
			using (_tcpClient)
				;
		}

		[Then]
		public void Should_establish_a_connection()
		{
			//_tcpClient.Connected.ShouldBeTrue();
		}

		[Then]
		public void Should_allow_multiple_connections()
		{
			List<TcpClient> clients = new List<TcpClient>();

			int expected = 100;
			for (int i = 0; i < expected - 1; i++)
			{
				var client = new TcpClient();
				clients.Add(client);

				client.Connect("localhost", 8008);
			}

			Thread.Sleep(50);

			_server.ConnectionCount.ShouldEqual(expected);

			clients.ForEach(client =>
				{
					using(client)
						client.Close();
				});

			clients.Clear();
		}
	}
}