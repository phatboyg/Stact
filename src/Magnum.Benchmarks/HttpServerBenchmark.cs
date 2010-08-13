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
namespace Magnum.Benchmarks
{
	using System;
	using Channels;
	using Fibers;
	using Servers;
	using Servers.Events;


	public class HttpServerBenchmark
	{
		public void Run()
		{
			var input = new ChannelAdapter();
			var connection = input.Connect(x =>
			{
				x.AddConsumerOf<ServerEvent>()
					.OnCurrentSynchronizationContext()
					.UsingConsumer(m => Console.WriteLine("Server " + m.EventType));
			});

			var serverUri = new Uri("http://localhost:8008/MagnumBenchmark");
			var server = new HttpServer(serverUri, new ThreadPoolFiber(), input, new[]
				{
					new VersionConnectionHandler(),
				});
			server.Start();

			Console.WriteLine("Started: press a key to shutdown");
			Console.ReadKey();

			server.Stop();

			Console.WriteLine("Stopping server");
			Console.ReadKey();
		}
	}
}