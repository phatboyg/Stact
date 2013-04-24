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
    public class When_a_behavior_has_an_exit_handler
    {
        [Test]
        public void Should_handle_the_exit()
        {
            _state.Received.WaitUntilCompleted(800.Seconds()).ShouldBeTrue();
        }

        MyState _state;

        [TestFixtureSetUp]
        public void Setup()
        {
            _state = new MyState();

            ActorRef agent = Actor.New(_state, x => x.ChangeBehavior<DefaultBehavior>());

            StatelessActor.New(actor => agent.Exit(actor.Self));
        }


        class MyState
        {
            public MyState()
            {
                Received = new Future<Message<Exit>>();
            }

            public Future<Message<Exit>> Received { get; set; }
        }


        class DefaultBehavior :
            Behavior<MyState>
        {
            readonly Actor<MyState> _actor;

            public DefaultBehavior(Actor<MyState> actor)
            {
                _actor = actor;
            }

            public void HandleExit(Message<Exit> message, NextExitHandler next)
            {
                Console.WriteLine("Intercepted exit!");

                _actor.State.Received.Complete(message);

                next(message);
            }
        }
    }
}