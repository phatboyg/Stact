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
	using Magnum;
	using Magnum.TestFramework;
	using Model;
	using Stact.Actors;


	[Scenario]
	public class When_adding_an_actor_to_a_registry
	{
		ActorFactory<Auction> _auctionFactory;
		ActorRegistry _registry;
		Guid _auctionId;

		[When]
		public void Adding_an_actor_to_a_registry()
		{
			_auctionFactory = ActorFactory.Create<Auction>(inbox => new Auction(inbox));

			_auctionId = CombGuid.Generate();

			var auction = _auctionFactory.GetActor();
			


		}
	}
}