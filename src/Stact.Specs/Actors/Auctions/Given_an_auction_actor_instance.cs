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
namespace Stact.Specs.Actors.Auctions
{
	using System;
	using Magnum;
	using Magnum.TestFramework;


	[Scenario]
	public class Given_an_auction_actor_instance
	{
		public ActorInstance Auction { get; private set; }
		public Guid Id { get; private set; }

		[Given]
		public void An_active_auction()
		{
			Id = CombGuid.Generate();

			ActorFactory<Auction> factory = ActorFactory.Create((f, i) => new Auction(f, i, Id));

			Auction = factory.GetActor();
		}

		[Finally]
		public void Finally()
		{
			Auction.Exit();
		}
	}
}