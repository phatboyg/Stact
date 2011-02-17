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
namespace Stact.Specs.Registries
{
	using System;
	using System.Collections.Generic;
	using Configuration;
	using Internal;
	using Magnum;
	using Magnum.TestFramework;
	using Model;
	using NUnit.Framework;
	using Stact.Actors;
	using Stact.Actors.Registries;


	[Scenario]
	public class When_adding_an_actor_to_a_registry
	{
		ActorFactory<Auction> _auctionFactory;
		Guid _auctionId;
		ActorRegistry _registry;

		public void Adding_an_actor_to_a_registry()
		{
			_auctionFactory = ActorFactory.Create(inbox => new Auction(inbox));

			_auctionId = CombGuid.Generate();

			ActorInstance auction = _auctionFactory.GetActor();

			ActorRegistry registry = new InMemoryActorRegistry(new PoolFiber());
			registry.Register(auction, (key, actor) => { });

			// need to proxy the channel with headers somehow... 

			// untyped channel => channel mapper -> actor instance

			// DestinationAddress -> set by outbound channel proxy on message<>
			// SourceAddress -> set by outbound channel proxy when available (not likely)
			// ResponseAddress -> set by outbound channel for ResponseChannel on Request to map to channel
			// Id -> system assigned id
			// DestinationAddress = urn:actor:554FC958-4661-4FE9-94F5-21D190417BCC
		}
	}

}