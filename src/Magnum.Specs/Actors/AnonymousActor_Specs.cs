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
namespace Magnum.Specs.Actors
{
	using Auctions;
	using Magnum.Actors;
	using Magnum.Channels;
	using Magnum.Extensions;
	using TestFramework;


	[Scenario]
	public class Sending_a_request_from_an_anonymous_actor :
		Given_an_auction_actor_instance
	{
		[Then]
		public void Should_receive_the_expected_value()
		{
			var response = new FutureChannel<Status>();

			ActorInstance instance = AnonymousActor.New(inbox =>
				{
					Auction.Request(new Ask(Id), inbox)
						//.Within(30.Seconds(), x =>
						//{
						.Receive<Response<Status>>(m => status => response.Complete(status.Body));
					//});
				});

			response.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("Timeout waiting for response");

			response.Value.AuctionId.ShouldEqual(Id);
		}
	}
}