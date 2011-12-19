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
    using System;
    using Magnum.Extensions;
    using NUnit.Framework;


    [TestFixture]
    public class The_actor_redesign_is_going_to_be_awesome
    {
        [Test]
        public void This_is_the_first_one()
        {
            ActorRef actor = Actor.New(new TestState(), x => x.Apply<DefaultBehavior>());
        }


        // you know there will be a state storage class at some point to allow
        // persistent state actors (sorta like sagas)
        class TestState
        {
            public string SomeValue;
        }

        /// <summary>
        /// since a behavior is a class, we can certainly inherit some basic behavior into each
        /// behavior so that we don't have to write it multiple times.
        /// </summary>
        abstract class BaseBehavior
        {
            public void Handle(Exception exception, NextExceptionHandler next)
            {
                // we call this if we want to let the next handler at it
                // might change to a cancel style or something else
                next(exception);
            }

            // just declaring this will intercept the exit method
            public virtual void HandleExit(ActorRef sender, NextExitHandler next)
            {
                // not calling the next handler means we never exit
            }
        }


        // behaviors would be created dynamically to handle the message so that
        // it could be lazy loaded for each message and constructed with the actor
        // instance
        class DefaultBehavior :
            BaseBehavior,
            Behavior<TestState>
        {
            readonly Actor<TestState> _actor;

            public DefaultBehavior(Actor<TestState> actor)
            {
                _actor = actor;
            }

            public void Handle(A message)
            {
                _actor.State.SomeValue = message.AValue;

                _actor.Apply<LimitedBehavior>();
            }

            public void Handle(ActorRef sender, B message)
            {
                _actor.Link(sender);
            }

            public void Handle(Message<C> message)
            {
                // message.Respond(new D());
            }
        }


        class LimitedBehavior :
            BaseBehavior,
            Behavior<TestState>
        {
            readonly Actor<TestState> _actor;

            public LimitedBehavior(Actor<TestState> actor)
            {
                _actor = actor;

                _actor.SetTimeout(30.Seconds(), HandleTimeout);
            }

            public void HandleTimeout()
            {
                _actor.Apply<DefaultBehavior>();
            }

            // of course i can do it in a behavior as well
            public override void HandleExit(ActorRef sender, NextExitHandler next)
            {
                next(sender);
            }
        }
    }


    class A
    {
        public string AValue { get; set; }
    }


    class B
    {
    }


    class C
    {
    }


    class D
    {
    }
}