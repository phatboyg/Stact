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
    using Magnum.Caching;
    using Magnum.Extensions;
    using Magnum.TestFramework;


    [Scenario]
    public class Using_an_anonymous_actor
    {
        [Then]
        public void Should_not_require_extensive_namespace_references()
        {
            var responded = new Future<MyResponse>();

            ActorRef server = StatelessActor.New(actor =>
                {
                    actor.Receive<MyRequest>(request =>
                        {
                            // send our response
                            request.Respond(new MyResponse());
                        });
                });

            ActorRef client = StatelessActor.New(actor =>
                {
                    server.Request(new MyRequest(), actor)
                        .ReceiveResponse<MyResponse>(response => responded.Complete(response.Body));
                });

            responded.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();

            server.Exit();
            client.Exit();
        }


        class MyRequest
        {
        }


        class MyResponse
        {
        }
    }


    class Cleaner_version_of_defining_actor_behavior
    {
        public void Some_method_in_your_code()
        {
            // create the actor instance, passing the state and an initializer
            // in this case, applying the initial behavior of to the actor
            ActorRef agent = Actor.New<AgentState>(x => x.ChangeTo<DefaultAgentBehavior>());

            // now messages can be sent to the actor to make it do things
            // we will use a stateless actor for that interaction
            StatelessActor.New(actor =>
            {
                agent.Request(new RequestStockQuote {Symbol = "AAPL"}, actor)
                    .ReceiveResponse<StockQuote>(response => { });
            });
        }

        // the state of an actor can be any type, and is accessible by the
        // behaviors applied to the actor


        class DefaultAgentBehavior :
            Behavior<AgentState>
        {
            readonly Actor<AgentState> _actor;

            // the actor is passed to the behavior, which gives it access 
            // to state. A new instance of the behavior class is created when
            // behavior is applies, and this executes within the actor execution
            // context
            public DefaultAgentBehavior(Actor<AgentState> actor)
            {
                _actor = actor;
            }

            // by a PublicMessageMethodConvention, this methods gets wired up
            // to the inbox to deliver messages of type A as part of the behavior
            public void Handle(Message<RequestStockQuote> message)
            {
                _actor.State.RequestCount++;

                var requestActor = Actor.New<RequestState>(x => x.ChangeTo<InitialRequestBehavior>());

                requestActor.Request(message.Body, _actor)
                    .ReceiveResponse<StockQuote>(quoteResponse =>
                        {
                            _actor.State.LastQuote[message.Body.Symbol] = quoteResponse.Body;
                            message.Respond(quoteResponse.Body);
                        });
            }

            // receiving a disable message will change the behavior of the actor
            public void Handle(Message<UseCache> message)
            {
                _actor.ChangeTo<CacheAgentBehavior>();
            }
        }


        // another behavior defined for the actor


        class CacheAgentBehavior :
            Behavior<AgentState>
        {
            readonly Actor<AgentState> _actor;

            public CacheAgentBehavior(Actor<AgentState> actor)
            {
                _actor = actor;
            }

            public void Handle(Message<RequestStockQuote> message)
            {
                _actor.State.RequestCount++;

                bool handled = _actor.State.LastQuote.WithValue(message.Body.Symbol, quote =>
                    {
                        _actor.State.CachedRequestCount++;

                        message.Respond(quote);
                    });

                if(!handled)
                {
                    // we may want to send a temporary unavailable response or something
                }
            }

            // in this case, handling the enable message changes back to the default
            // behavior
            public void Handle(Message<UseLive> message)
            {
                _actor.ChangeTo<DefaultAgentBehavior>();
            }
        }


        class AgentState
        {
            public AgentState()
            {
                LastQuote = new DictionaryCache<string, StockQuote>();
            }

            public int RequestCount { get; set; }
            public int CachedRequestCount { get; set; }
            public Cache<string, StockQuote> LastQuote { get; set; }
        }


        class RequestState
        {
            public string Symbol { get; set; }
        }


        class InitialRequestBehavior :
            Behavior<RequestState>
        {
            readonly Actor<RequestState> _actor;
            readonly ISessionFactory _sessionFactory;

            public InitialRequestBehavior(Actor<RequestState> actor, ISessionFactory sessionFactory)
            {
                _actor = actor;
                _sessionFactory = sessionFactory;
            }

            public void Handle(Message<RequestStockQuote> message)
            {
                _actor.State.Symbol = message.Body.Symbol;

                // we do some expensive work to get the quote value
                using(var session = _sessionFactory.OpenSession())
                {
                    var stock = session.Load<Stock>(message.Body.Symbol);
                    message.Respond(new StockQuote
                        {
                            LastPrice = stock.LastPrice
                        });
                }
            }
        }


        class RequestStockQuote
        {
            public string Symbol { get; set; }
        }


        class StockQuote
        {
            public decimal LastPrice { get; set; }
        }


        class UseCache
        {
        }


        class UseLive
        {
        }


        class Stock
        {
            public decimal LastPrice { get; set; }
        }


        interface ISessionFactory
        {
            ISession OpenSession();
        }


        interface ISession :
            IDisposable
        {
            T Load<T>(object key);
        }
    }
}