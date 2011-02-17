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
	using System.Diagnostics;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using Magnum;
	using Magnum.Extensions;
	using Magnum.TestFramework;


	[Scenario]
	public class Using_pgm_for_remote_actors
	{
		[Then]
		public void Might_be_a_good_idea()
		{
			using (var server = new Socket(AddressFamily.InterNetwork, SocketType.Rdm, (ProtocolType)113))
			{
				IPAddress localAddress = IPAddress.Any;
				var localEndpoint = new IPEndPoint(localAddress, 0);

				server.Bind(localEndpoint);

				IPAddress multicastAddress = IPAddress.Parse("227.5.4.9");
				var multicastEndpoint = new IPEndPoint(multicastAddress, 9000);

				var ep = (EndPoint)multicastEndpoint;

				// connect socket to multicast address group
				server.Connect(ep);


				using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Rdm, (ProtocolType)113))
				{
					client.Bind(ep);
					client.Listen(1000);

					AsyncCallback acceptor = null;
					acceptor = x =>
						{
						};

					client.BeginAccept(x =>
						{
							Socket socket = client.EndAccept(x);

							var buffer = new byte[32768];

							AsyncCallback receiver = null;
							receiver = r =>
								{
									int receiveLength = socket.EndReceive(r);
									if (receiveLength == 0)
										return;

									string text = Encoding.UTF8.GetString(buffer, 0, receiveLength);
									Trace.WriteLine("Received: " + text);

									socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, receiver, socket);
								};


							client.BeginAccept(acceptor, client);
						}, client);

					string output = "Hello, World.";
					byte[] block = Encoding.UTF8.GetBytes(output);

					server.Send(block, 0, block.Length, SocketFlags.None);

					ThreadUtil.Sleep(5.Seconds());
				}
			}
		}
	}
}