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
namespace Stact.Specs.Example
{
	public class Auction :
		Actor
	{
		decimal _currentBid;
		UntypedChannel _highBidder;
		bool _open = true;

		public Auction(Inbox inbox, Fiber fiber, Scheduler scheduler)
		{
			_currentBid = 1.00m;

			scheduler.Schedule(60000, fiber, () =>
			{
				inbox.Send(new EndAuction());
			});

			inbox.Loop(loop =>
			{
				loop.Receive<Request<AuctionStatus>>(request =>
				{
					request.Respond(new AuctionStatusImpl
					{
						CurrentBid = _currentBid,
						IsOpen = _open,
					});
				});

				loop.Receive<Request<PlaceBid>>(request =>
				{
					if (request.Body.MaximumBid <= _currentBid)
					{
						request.Respond(new OutBidImpl
						{
							CurrentBid = _currentBid
						});
					}
					else
					{
						_currentBid = request.Body.MaximumBid;
						request.Respond(new BidAcceptedImpl
						{
							CurrentBid = _currentBid
						});
						if (_highBidder != null)
						{
							_highBidder.Send(new OutBidImpl
							{
								CurrentBid = _currentBid
							});
						}
						_highBidder = request.ResponseChannel;
					}

					loop.Continue();
				});

				loop.Receive<EndAuction>(msg =>
				{
					_open = false;
//					_highBidder.Send(new YouWin());
				});
			});
		}
	}


	public class EndAuction {}


	public interface AuctionStatus
	{
		decimal CurrentBid { get; }
	}


	class AuctionStatusImpl : AuctionStatus
	{
		public bool IsOpen { get; set; }
		public decimal CurrentBid { get; set; }
	}


	public interface BidAccepted :
		AuctionStatus {}


	class BidAcceptedImpl : BidAccepted
	{
		public decimal CurrentBid { get; set; }
	}


	public interface PlaceBid
	{
		decimal MaximumBid { get; }
	}


	public interface OutBid
	{
		decimal CurrentBid { get; }
	}


	public class OutBidImpl :
		OutBid
	{
		public decimal CurrentBid { get; set; }
	}
}