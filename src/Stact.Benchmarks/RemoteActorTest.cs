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
	using Magnum;
	using Magnum.Extensions;
	using Remote;


	public class RemoteActorTest
	{
		public void Run()
		{
			const string remoteAddress = "rm://234.0.0.7:40001/";

			Guid id = CombGuid.Generate();

			var registry = (RemoteActorRegistry)ActorRegistryFactory.New(x =>
				{
					x.Remote(r => r.ListenTo(remoteAddress));
				});

			var server = AnonymousActor.New(inbox =>
				{
					inbox.Receive<Hello>(message =>
						{
							Console.WriteLine("Hi!");
						});
				});


			registry.Register(id, server);

			ActorInstance actor = registry.Select(new Uri(remoteAddress + id.ToString("N")));

			actor.Send(new Hello
				{
					MyNameIs = "Joe",
				});

			ThreadUtil.Sleep(5.Seconds());

			actor.Exit();

			registry.Shutdown();
		}


		public class Hello
		{
			public string MyNameIs { get; set; }
		}
	}
}