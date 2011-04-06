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
namespace Stact.ServerFramework.Specs
{
	using System;
	using System.Collections.Generic;
	using System.Net.Sockets;
	using System.Threading;
	using Events;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;


	[Scenario]
	public class When_starting_a_socket_server
	{
		ChannelConnection _connection;
		ChannelAdapter _input;
		Future<ServerRunning> _runningEventReceived;
		protected SocketServer _server;
		Future<ServerStarting> _startingEventReceived;
		Uri _uri;

		[When]
		public void Starting_a_socket_server()
		{
			_startingEventReceived = new Future<ServerStarting>();
			_runningEventReceived = new Future<ServerRunning>();

			_input = new ChannelAdapter();
			_connection = _input.Connect(x =>
			{
				x.AddConsumerOf<ServerStarting>()
					.UsingConsumer(_startingEventReceived.Complete)
					.HandleOnCallingThread();

				x.AddConsumerOf<ServerRunning>()
					.UsingConsumer(_runningEventReceived.Complete)
					.HandleOnCallingThread();
			});

			_uri = new Uri("tcp://0.0.0.0:8008");

			_server = new SocketServer(_uri, _input);
			_server.Start();
		}

		[After]
		public void Finally()
		{
			_server.Stop();

			_connection.Dispose();
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
	[Explicit]
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
			var clients = new List<TcpClient>();

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
				using (client)
					client.Close();
			});

			clients.Clear();
		}
	}
}