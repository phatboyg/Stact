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
    using System;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_behavior_has_an_exception_handler
    {
        [Test]
        public void Should_receive_the_response()
        {
            _state.Received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();

            Assert.IsInstanceOf<InvalidOperationException>(_state.Received.Value);
        }

        MyState _state;

        [TestFixtureSetUp]
        public void Setup()
        {
            _state = new MyState();

            ActorRef agent = Actor.New(_state, x => x.Apply<DefaultBehavior>());

            StatelessActor.New(actor => agent.Send(new A().ToMessage(actor.Self)));
        }


        class MyState
        {
            public MyState()
            {
                Received = new Future<Exception>();
            }

            public Future<Exception> Received { get; set; }
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
                throw new InvalidOperationException("This is expected, but should be handled.");
            }

            public void HandleException(Exception exception, NextExceptionHandler next)
            {
                _actor.State.Received.Complete(exception);

                next(exception);
            }
        }


        class A
        {
        }
    }
}