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
namespace Stact.Specs.Workflow
{
	using System;
	using System.Diagnostics;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using Stact.Workflow;


	[Scenario]
	public class StateMachineActor_Specs
	{
		[Then]
		public void Easy_syntax_love()
		{
			ActorFactory<QuoteService> factory =
				StateMachineActorFactory.Create<QuoteServiceWorkflow, QuoteService>(inbox => new QuoteService(inbox), x =>
					{
						x.AccessCurrentState(s => s.CurrentState);

						x.Initially()
							.When(e => e.GetQuoteRequest)
							.Then(i => i.SendQuoteRequest)
							.Respond(() => new RequestSent())
							.TransitionTo(s => s.WaitingForResponse);

						x.During(s => s.WaitingForResponse)
							.When(e => e.CancelRequest)
							.Then(i => i.Cancel)
							.Respond(() => new CancelledImpl());
					});

			ActorInstance service = factory.GetActor();


			var response = new Future<Response<RequestSent>>();

			AnonymousActor.New(inbox =>
				{
					service.Request(new GetQuote
						{
							Symbol = "MSFT"
						}, inbox)
						.Receive<Response<RequestSent>>(r => response.Complete(r));

				});

			service.Exit();

			response.WaitUntilCompleted(5.Seconds()).ShouldBeTrue();
		}



		class RequestSent
		{
			
		}
		interface Cancel
		{
		}


		interface Cancelled
		{
		}


		internal class CancelledImpl : Cancelled
		{
		}


		class GetQuote
		{
			public string Symbol { get; set; }
		}

		interface InvalidSymbol
		{
		}


		interface Quote
		{
		}


		class QuoteService :
			Actor
		{
			public QuoteService(Inbox inbox)
			{
			}

			public State CurrentState { get; private set; }

			public void SendQuoteRequest(Request<GetQuote> request)
			{
				Symbol = request.Body.Symbol;
			}

			public string Symbol { get; set; }

			public void Cancel(Request<Cancel> request)
			{
				request.Respond(new CancelledImpl());
			}
		}


		interface QuoteServiceWorkflow
		{
			State WaitingForResponse { get; }

			Event<Request<GetQuote>> GetQuoteRequest { get; }
			Event<Request<Cancel>> CancelRequest { get; }
		}
	}
}