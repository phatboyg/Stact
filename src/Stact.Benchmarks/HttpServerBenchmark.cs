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
namespace Stact.Benchmarks
{
	using System;
	using Channels;
	using Fibers;
	using Servers;
	using Servers.Events;
	using ValueProviders;


	public class HttpServerBenchmark
	{
		public void Run()
		{
			var provider = new CommandLineValueProvider();

			var buildUri = new UriBuilder("http://localhost:8008/StactBenchmark");

			provider.GetValue("port", x =>
				{
					buildUri.Port = int.Parse(x.ToString());

					return true;
				});

			provider.GetValue("path", x =>
				{
					buildUri.Path = x.ToString();
					return true;
				});

			var input = new ChannelAdapter();
			ChannelConnection connection = input.Connect(x =>
				{
					x.AddConsumerOf<ServerEvent>()
						.OnCurrentSynchronizationContext()
						.UsingConsumer(m => Console.WriteLine("Server " + m.EventType));
				});

			var serverUri = buildUri.Uri;
			Console.WriteLine("Using server uri: " + serverUri);
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