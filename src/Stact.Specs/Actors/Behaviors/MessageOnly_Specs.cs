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
    using Configuration;
    using Configuration.Conventions;
    using NUnit.Framework;
    using Stact.Behaviors;


    [TestFixture]
    public class When_a_behavior_has_only_a_message_in_the_handler
    {
        [Test]
        public void Should_have_a_binder_for_the_handler()
        {
            var conventions = new BehaviorConvention[]
                {
                    new MessageOnlyMethodBehaviorConvention(), 
                };

            ActorBehaviorFactory<TestState> factory =
                new ActorBehaviorFactoryImpl<TestState>(conventions);

            ActorBehavior<TestState> instance = factory.CreateActorBehavior<TestBehavior>();
        }


        class TestBehavior :
            Behavior<TestState>
        {
            readonly Actor<TestState> _actor;

            public TestBehavior(Actor<TestState> actor)
            {
                _actor = actor;
            }

            public void Handle(Message<A> message)
            {
                _actor.State.Value = message.Body.Value;
            }
        }


        class A
        {
            public string Value { get; set; }
        }


        class TestState
        {
            public string Value { get; set; }
        }
    }

    [TestFixture]
    public class When_a_behavior_has_a_message_and_exception_handler
    {
        [Test]
        public void Should_have_a_binder_for_the_handler()
        {
            var conventions = new BehaviorConvention[]
                {
                    new MessageOnlyMethodBehaviorConvention(), 
                    new ExceptionHandlerMethodBehaviorConvention(), 
                };

            ActorBehaviorFactory<TestState> factory =
                new ActorBehaviorFactoryImpl<TestState>(conventions);

            ActorBehavior<TestState> instance = factory.CreateActorBehavior<TestBehavior>();
        }


        class TestBehavior :
            Behavior<TestState>
        {
            readonly Actor<TestState> _actor;

            public TestBehavior(Actor<TestState> actor)
            {
                _actor = actor;
            }

            public void Handle(Message<A> message)
            {
                _actor.State.Value = message.Body.Value;
            }

            public void HandleException(Exception exception, NextExceptionHandler next)
            {
                next(exception);
            }
        }


        class A
        {
            public string Value { get; set; }
        }


        class TestState
        {
            public string Value { get; set; }
        }
    }
}