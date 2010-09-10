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
	using Magnum.Channels;
	using Magnum.Channels.Context;


	public class Auction :
		Actor
	{
		readonly Fiber _fiber;
		readonly Inbox _inbox;
		readonly Scheduler _scheduler;

		public Auction(Fiber fiber, Scheduler scheduler)
		{
			_fiber = fiber;
			_scheduler = scheduler;
	
			AskChannel = new SelectiveConsumerChannel<Request<Ask>>(_fiber, HandleAsk);

			_inbox = new ActorInbox<Auction>(_fiber, _scheduler, this);

		}

		Consumer<Request<Ask>> HandleAsk(Request<Ask> x)
		{
			if (x.Body.AuctionId != Id)
				return null;

			return message =>
				{
					x.Respond(new Status
						{
							CurrentBid = 1.00m,
							AuctionId = Id,
						});
				};
		}

		public Guid Id { get; private set; }
		public Channel<Request<Ask>> AskChannel { get; private set; }

		public void Send<T>(T message)
		{
			_inbox.Send(message);
		}
	}


	public class Ask
	{
		public Ask(Guid id)
		{
			AuctionId = id;
		}

		public Guid AuctionId { get; set; }
	}


	public class Status
	{
		public Guid AuctionId { get; set; }
		public decimal CurrentBid { get; set; }
	}
}