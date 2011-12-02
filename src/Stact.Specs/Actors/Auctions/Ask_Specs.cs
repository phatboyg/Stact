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
    using Stact;
    using Magnum.Extensions;
    using Magnum.TestFramework;


    [Scenario]
    public class When_a_bid_is_sent_to_the_actor_instance :
        Given_an_auction_actor_instance
    {
        [Then]
        public void Sending_a_bid_request_should_get_a_response()
        {
            var response = new FutureChannel<Message<Status>>();

            UntypedChannel responseChannel = new ChannelAdapter();
            responseChannel.Connect(x => x.AddChannel(response));

            // Auction.Request(new Ask(Id).ToMessage(), responseChannel);

            response.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("Timeout waiting for response");

            response.Value.Body.AuctionId.ShouldEqual(Id);


            // ThreadUtil.Sleep(2.Seconds());
            // go ahead and buy something

            var purchased = new FutureChannel<Message<Purchased>>();

            responseChannel.Connect(x => x.AddChannel(purchased));

//            Auction.Request(new Buy
//                {
//                    Quantity = 15,
//                    Token = response.Value.Body.Token
//                }.ToMessage(), responseChannel);

            purchased.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("Timeout waiting for purchase");

            purchased.Value.Body.Quantity.ShouldEqual(15);
            purchased.Value.Body.Price.ShouldEqual(response.Value.Body.CurrentBid);
        }
    }
}