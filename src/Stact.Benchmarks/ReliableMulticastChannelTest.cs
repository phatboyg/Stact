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
	using System.Threading;
	using Internal;
	using Magnum.Serialization;
	using MessageHeaders;
	using Remote;
	using Remote.ReliableMulticast;


	public class ReliableMulticastChannelTest
	{
		public void Run()
		{
			var network = new Uri("pgm://224.0.0.7:40001");

			ActorInstance actor = AnonymousActor.New(inbox =>
				{
					inbox.Loop(loop =>
						{
							loop
								.Receive<A>(message =>
									{
										Console.WriteLine("Received: " + message.Name);

										loop.Repeat();
									})
								.Receive<B>(message =>
									{
										Console.WriteLine("Received: " + message.Address);
										loop.Repeat();
									});
						});
				});

			using (var writer = new ReliableMulticastWriter(network))
			{
				writer.Start();

				MessageHeaders.MatchHeaderChannel channel;
				using (var buffer = new BufferedChunkWriter(new PoolFiber(), new TimerScheduler(new PoolFiber()), writer, 64*1024))
				{
					buffer.Start();

					channel = new MessageHeaders.MatchHeaderChannel(new Remote.MatchHeaderChannel(new SerializeChunkChannel(buffer, new FastTextSerializer())));


					MessageHeaders.MatchHeaderChannel channel2;
					using (var buffer2 = new BufferedChunkWriter(new PoolFiber(), new TimerScheduler(new PoolFiber()), writer, 64*1024)
						)
					{
						buffer2.Start();
						channel2 = new MessageHeaders.MatchHeaderChannel(new Remote.MatchHeaderChannel(new SerializeChunkChannel(buffer2, new FastTextSerializer())));

						Console.WriteLine("Writer started");

						for (int i = 0; i < 10; i++)
						{
							channel.Send(new A
								{
									Name = "Joe"
								});

							channel2.Send(new B
								{
									Address = "American Way",
								});
						}
						Console.WriteLine("Sent message");

						var reader = new DeserializeChunkChannel(actor, new FastTextSerializer());

						using (var listener = new ReliableMulticastListener(network, reader))
						{
							Console.WriteLine("Listener created");
							listener.Start();

							Console.WriteLine("Listener started");

							Thread.Sleep(2000);

							Console.WriteLine("Leaving Listener");
						}

						Console.WriteLine("Listener stopped");
					}
				}
			}

			Console.WriteLine("Sender Stopped");
		}


		public class A
		{
			public string Name { get; set; }
		}


		public class B
		{
			public string Address { get; set; }
		}
	}
}