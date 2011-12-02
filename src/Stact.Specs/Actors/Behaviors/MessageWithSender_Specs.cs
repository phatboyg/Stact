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
namespace Stact.Specs.Actors.Behaviors
{
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_behavior_accepts_a_message_with_the_sender
    {
        [Test]
        public void Should_receive_the_response()
        {
            _responseReceived.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
        }


        Future<B> _responseReceived;

        [TestFixtureSetUp]
        public void Setup()
        {
            _responseReceived = new Future<B>();

            var state = new MyState();

            ActorRef agent = Actor.New(state, x => x.Apply<DefaultBehavior>());

            StatelessActor.New(actor =>
                {
                    agent.Send(new A
                        {
                            Value = "Hello"
                        }.ToMessage(actor.Self));

                    actor.Receive<B>(response => _responseReceived.Complete(response));
                });
        }


        class MyState
        {
        }


        class DefaultBehavior :
            Behavior<MyState>
        {
            readonly Actor<MyState> _actor;

            public DefaultBehavior(Actor<MyState> actor)
            {
                _actor = actor;
            }

            public void Handle(ActorRef sender, A message)
            {
                sender.Send(new B
                    {
                        Value = message.Value
                    }.ToMessage(_actor.Self));
            }
        }


        class A
        {
            public string Value { get; set; }
        }


        class B
        {
            public string Value { get; set; }
        }
    }

    [TestFixture]
    public class When_a_behavior_accepts_a_message_header_with_the_sender
    {
        [Test]
        public void Should_receive_the_response()
        {
            _responseReceived.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
        }


        Future<B> _responseReceived;

        [TestFixtureSetUp]
        public void Setup()
        {
            _responseReceived = new Future<B>();

            var state = new MyState();

            ActorRef agent = Actor.New(state, x => x.Apply<DefaultBehavior>());

            StatelessActor.New(actor =>
                {
                    agent.Send(new A
                        {
                            Value = "Hello"
                        }.ToMessage(actor.Self));

                    actor.Receive<B>(response => _responseReceived.Complete(response));
                });
        }


        class MyState
        {
        }


        class DefaultBehavior :
            Behavior<MyState>
        {
            readonly Actor<MyState> _actor;

            public DefaultBehavior(Actor<MyState> actor)
            {
                _actor = actor;
            }

            public void Handle(ActorRef sender, Message<A> message)
            {
                sender.Send(new B
                    {
                        Value = message.Body.Value
                    }.ToMessage(_actor.Self));
            }
        }


        class A
        {
            public string Value { get; set; }
        }


        class B
        {
            public string Value { get; set; }
        }
    }
}
