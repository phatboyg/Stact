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
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using Model;


	[Scenario]
	public class When_looping_within_an_actor_via_the_inbox
	{
		[Then]
		public void Should_support_the_repeat_syntax()
		{
			var completed = new Future<Status>();

			ActorInstance auction = AnonymousActor.New(inbox =>
				{
					decimal currentBid = 0.0m;

					inbox.Repeat()
						.Receive<Request<Bid>>(request =>
							{
								if (request.Body.MaximumBid > currentBid)
									currentBid = request.Body.MaximumBid;

								request.Respond(new StatusImpl
									{
										CurrentBid = currentBid
									});
							})
						.Receive<Request<Ask>>(request =>
							{
								request.Respond(new StatusImpl
									{
										CurrentBid = currentBid
									});
							});
				});

			ActorInstance bidder = AnonymousActor.New(inbox =>
				{
					auction.Request(new BidImpl(13.5m), inbox)
						.Receive<Response<Status>>(bidResponse =>
							{
								auction.Request<Ask>(inbox)
									.Receive<Response<Status>>(askResponse => completed.Complete(askResponse.Body));
							});
				});

			completed.WaitUntilCompleted(500.Seconds()).ShouldBeTrue();

			bidder.Exit();
			auction.Exit();
		}
	}
}