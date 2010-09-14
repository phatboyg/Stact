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
namespace Magnum.Specs.Actors.Auctions
{
	using System;
	using Fibers;
	using Magnum.Actors;
	using Magnum.Actors.Internal;
	using Magnum.Channels;
	using Magnum.Channels.Context;
	using Magnum.Extensions;
	using TestFramework;


	[Scenario]
	public class Ask_Specs
	{
		ActorInstance _auction;
		UntypedChannel _responseChannel;
		Guid _id;

		[Given]
		public void An_active_auction()
		{
			_id = CombGuid.Generate();

			var factory = new DelegateActorFactory<Auction>(() => new ThreadPoolFiber(),
			                                                () => new TimerScheduler(new ThreadPoolFiber()),
			                                                (f, s, i) => new Auction(f, i, _id));

			_auction = factory.GetActor();
		}

		[Finally]
		public void Finally()
		{
			_auction.Exit();
		}

		[Then]
		public void Sending_a_bid_request_should_get_a_response()
		{
			var response = new FutureChannel<Response<Status>>();

			_responseChannel = new ChannelAdapter();
			_responseChannel.Connect(x => x.AddChannel(response));

			_auction.Request(new Ask(_id), _responseChannel);

			response.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("Timeout waiting for response");

			response.Value.Body.AuctionId.ShouldEqual(_id);


			// ThreadUtil.Sleep(2.Seconds());
			// go ahead and buy something

			var purchased = new FutureChannel<Response<Purchased>>();

			_responseChannel.Connect(x => x.AddChannel(purchased));

			_auction.Request(new Buy
				{
					Quantity = 15,
					Token = response.Value.Body.Token
				}, _responseChannel);

			purchased.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("Timeout waiting for purchase");

			purchased.Value.Body.Quantity.ShouldEqual(15);
			purchased.Value.Body.Price.ShouldEqual(response.Value.Body.CurrentBid);
		}
	}
}