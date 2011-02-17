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
namespace Stact.Benchmarks
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using Magnum;
	using Magnum.Extensions;


	public class PgmTest
	{
		public void Run()
		{
			using (var server = new Socket(AddressFamily.InterNetwork, SocketType.Rdm, (ProtocolType)113))
			{
				IPEndPoint duh =  new Uri("tcp://0.0.0.0:9000").ResolveHostName().Single();

				Console.WriteLine("IPAddress: " + duh);
				IPAddress localAddress = IPAddress.Parse("127.0.0.1");
				var localEndpoint = new IPEndPoint(localAddress, 0);

				server.Bind(duh);

				IPAddress multicastAddress = IPAddress.Parse("234.0.0.7");
				var multicastEndpoint = new IPEndPoint(multicastAddress, 9000);

				var ep = (EndPoint)multicastEndpoint;

				// connect socket to multicast address group
				server.Connect(ep);


				using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Rdm, (ProtocolType)113))
				{
					client.Bind(ep);
					client.Listen(1000);

					Console.WriteLine("Listing");

					AsyncCallback acceptor = null;
					acceptor = x =>
						{
							Console.WriteLine("Accepting");

							try
							{
								Socket socket = client.EndAccept(x);

								Console.WriteLine("Accepted");

								var buffer = new byte[32768];

								AsyncCallback receiver = null;
								receiver = r =>
									{
										Console.WriteLine("Receiving");

										int receiveLength = socket.EndReceive(r);
										if (receiveLength == 0)
										{
											socket.Close();
											socket.Dispose();
										}

										Console.WriteLine("Received");

										string text = Encoding.UTF8.GetString(buffer, 0, receiveLength);
										Console.WriteLine("Received: " + text);

										socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, receiver, socket);
									};

								ThreadPool.QueueUserWorkItem(cb =>
									{
										socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, receiver, socket);
									});

								client.BeginAccept(acceptor, client);
							}
							catch (ObjectDisposedException ex)
							{
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex);
							}
						};

					client.BeginAccept(acceptor, client);

					string output = "Hello, World.";
					byte[] block = Encoding.UTF8.GetBytes(output);

					server.Send(block, 0, block.Length, SocketFlags.None);

					Console.WriteLine("Sent block");

					ThreadUtil.Sleep(10.Seconds());

					Console.WriteLine("Exiting");

					client.Close();

					server.Close();
				}
			}
		}
	}
}