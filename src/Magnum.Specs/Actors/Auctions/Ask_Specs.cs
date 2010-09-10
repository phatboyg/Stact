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
	using Fibers;
	using Magnum.Channels;
	using Magnum.Channels.Context;
	using Magnum.Extensions;
	using TestFramework;


	[Scenario]
	public class Ask_Specs
	{
		Auction _auction;
		UntypedChannel _responseChannel;

		[Given]
		public void An_active_auction()
		{
			_auction = new Auction(new ThreadPoolFiber(), new TimerScheduler(new ThreadPoolFiber()));
		}

		[Then]
		public void Sending_a_bid_request_should_get_a_response()
		{
			var response = new FutureChannel<Response<Status>>();

			_responseChannel = new ChannelAdapter();
			_responseChannel.Connect(x => x.AddChannel(response));

			_auction.Request(new Ask(_auction.Id), _responseChannel);

			response.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("Timeout waiting for response");

			response.Value.Body.AuctionId.ShouldEqual(_auction.Id);
		}
	}
}