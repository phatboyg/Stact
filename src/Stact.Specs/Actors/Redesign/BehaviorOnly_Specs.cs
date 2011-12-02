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
namespace Stact.Specs.Actors.Redesign
{
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;


    [TestFixture]
    public class When_an_actor_has_behavior
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

            ActorRef agent = Actor.New(state, x => x.Apply<DefaultBehavior>());

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