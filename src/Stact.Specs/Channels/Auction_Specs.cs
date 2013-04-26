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
namespace Stact.Specs.Channels
{
	using System;
	using Magnum;
	using Magnum.TestFramework;


	[Scenario]
	public class Given_an_auction
	{
		protected class Auction
		{
			public Auction(Guid id)
			{
				Id = id;

                Fiber = new TaskFiber();

				BidChannel = new ConsumerChannel<Bid>(Fiber, ReceiveBid);
			}

			public Guid Id { get; private set; }
			public decimal HighBid { get; private set; }

			public Fiber Fiber { get; private set; }
			public Channel<Bid> BidChannel { get; private set; }

			void ReceiveBid(Bid bid)
			{
				if (bid.Amount > HighBid)
					HighBid = bid.Amount;
			}
		}


		protected class Bid
		{
			public Guid Id { get; private set; }
			public Guid AuctionId { get; private set; }
			public decimal Amount { get; private set; }
		}
	}


	[Scenario]
	public class When_a_message_is_sent_to_an_instance :
		Given_an_auction
	{
		Guid _auctionId;
		ChannelConnection _connection;
		ChannelAdapter _input;

		[When]
		public void A_message_is_sent_to_an_instance()
		{
			_auctionId = CombGuid.Generate();

			_input = new ChannelAdapter();
			_connection = _input.Connect(x =>
			{
				x.AddConsumerOf<Bid>()
					.UsingInstance().Of<Auction>()
					.ObtainedBy(m => new Auction(m.AuctionId))
					.OnChannel(auction => auction.BidChannel);
			});
		}

		[After]
		public void After()
		{
			_connection.Dispose();
			_connection = null;
			_input = null;
		}
	}
}