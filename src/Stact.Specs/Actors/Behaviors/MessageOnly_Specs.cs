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
    public class When_a_behavior_accepts_a_raw_message
    {
        [Test]
        public void Should_receive_the_response()
        {
            _state.Received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
        }

        MyState _state;

        [TestFixtureSetUp]
        public void Setup()
        {
            _state = new MyState();

            ActorRef agent = Actor.New(_state, x => x.ChangeTo<DefaultBehavior>());

            StatelessActor.New(actor => agent.Send(new A(), actor.Self));
        }


        class MyState
        {
            public MyState()
            {
                Received = new Future<A>();
            }

            public Future<A> Received { get; set; }
        }

        class DefaultBehavior :
            Behavior<MyState>
        {
            readonly Actor<MyState> _actor;

            public DefaultBehavior(Actor<MyState> actor)
            {
                _actor = actor;
            }

            public void Handle(A message)
            {
                _actor.State.Received.Complete(message);
            }
        }


        class A
        {
        }
    }

    [TestFixture]
    public class When_a_behavior_accepts_a_message_header
    {
        [Test]
        public void Should_receive_the_response()
        {
            _receivedB.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
        }

        Future<B> _receivedB;

        [TestFixtureSetUp]
        public void Setup()
        {
            _receivedB = new Future<B>();

            var state = new MyState();

            ActorRef agent = Actor.New(state, x => x.ChangeTo<DefaultBehavior>());

            StatelessActor.New(actor =>
                {
                    agent.Request(new A(), actor)
                        .ReceiveResponse<B>(response => _receivedB.Complete(response));
                });
        }


        class MyState
        {
            public int RequestCount { get; set; }
        }


        class DefaultBehavior :
            Behavior<MyState>
        {
            readonly Actor<MyState> _actor;

            public DefaultBehavior(Actor<MyState> actor)
            {
                _actor = actor;
            }

            public void Handle(Message<A> message)
            {
                _actor.State.RequestCount++;
                message.Respond(new B());
            }
        }


        class A
        {
        }


        class B
        {
        }
    }
}