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
namespace Stact.Specs.Actors
{
	using System;
	using Auctions;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using Stact;


	[Scenario]
	public class Sending_a_request_from_an_anonymous_actor :
		Given_an_auction_actor_instance
	{
		[Then]
		public void Should_receive_the_expected_value()
		{
			var response = new FutureChannel<Status>();

			var actor = AnonymousActor.New(inbox =>
				{
					Auction.Request(new Ask(Id), inbox)
						.Within(30.Seconds())
						.Receive<Response<Status>>(m => status => response.Complete(status.Body))
						.Receive<Response<Ended>>(m => ended => { });
				});

			response.WaitUntilCompleted(4.Seconds()).ShouldBeTrue("Timeout waiting for response");

			response.Value.AuctionId.ShouldEqual(Id);
		}
	}


	[Scenario]
	public class Sending_a_request_from_an_anonymous_actor_to_an_ended_auction :
		Given_an_auction_actor_instance
	{
		[Then]
		public void Should_receive_the_alternate_ending_if_it_is_such()
		{
			Auction.Send(new End());

			var response = new FutureChannel<Ended>();

			var actor = AnonymousActor.New(inbox =>
				{
					Auction.Request(new Ask(Id), inbox)
						.Within(30.Seconds())
						.Receive<Response<Status>>(m => status => { })
						.Receive<Response<Ended>>(m => ended => response.Complete(ended.Body));
				});

			response.WaitUntilCompleted(4.Seconds()).ShouldBeTrue("Timeout waiting for response");

			response.Value.AuctionId.ShouldEqual(Id);
		}
	}


	[Scenario]
	public class Sending_another_request_inside_a_receive :
		Given_an_auction_actor_instance
	{
		[Then]
		public void Should_properly_handle_the_response()
		{
			var response = new FutureChannel<Purchased>();
			decimal price = 0.0m;

			var actor = AnonymousActor.New(inbox =>
				{
					Auction.Request(new Ask(Id), inbox)
						.Within(30.Seconds())
						.Receive<Response<Status>>(m => status =>
							{
								price = status.Body.CurrentBid;
								Auction.Request(new Buy(status.Body.Token, 1), inbox)
									.Within(30.Seconds())
									.Receive<Response<Purchased>>(pm => pmsg => response.Complete(pmsg.Body))
									.Otherwise(() => { });
							})
						.Receive<Response<Ended>>(m => ended => { });
				});

			response.WaitUntilCompleted(4.Seconds()).ShouldBeTrue("Timeout waiting for response");

			response.Value.AuctionId.ShouldEqual(Id);
			response.Value.Quantity.ShouldEqual(1);
			response.Value.Price.ShouldEqual(price);
		}
	}


	[Scenario]
	public class When_one_receive_handler_has_been_called :
		Given_an_auction_actor_instance
	{
		[Then]
		public void Should_not_call_the_other_receive_handlers()
		{
			var statusResponse = new FutureChannel<Status>();
			var endedResponse = new FutureChannel<Ended>();

			var actor = AnonymousActor.New(inbox =>
				{
					Auction.Request(new Ask(Id), inbox)
						.Within(10.Seconds())
						.Receive<Response<Status>>(m => status =>
							{
								statusResponse.Complete(status.Body);
								Auction.Send(new End());
								Auction.Request(new Ask(Id), inbox);
							})
						.Receive<Response<Ended>>(m => ended => endedResponse.Complete(ended.Body));
				});

			statusResponse.WaitUntilCompleted(4.Seconds()).ShouldBeTrue("Timeout waiting for response");
			endedResponse.WaitUntilCompleted(2.Seconds()).ShouldBeFalse("The receiver for Ended should not have been called.");
		}
	}


	[Scenario]
	public class Sending_a_request_from_an_anonymous_actor_to_an_auction :
		Given_an_auction_actor_instance
	{
		[Then]
		public void Should_not_receive_a_response_to_a_stupid_message()
		{
			var response = new FutureChannel<bool>();

			var actor = AnonymousActor.New(inbox =>
				{
					Auction.Request(new Ask(new Guid()), inbox)
						.Within(1.Seconds())
						.Receive<Response<Status>>(m => status => { })
						.Otherwise(() => response.Complete(true));
				});

			response.WaitUntilCompleted(4.Seconds()).ShouldBeTrue("Timeout waiting for otherwise to be called");
		}
	}
}