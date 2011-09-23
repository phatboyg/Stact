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
    using System.Threading;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using Routing.Visualizers;


    [Scenario]
    public class When_an_exit_is_sent_to_an_actor_that_intercepts_exit
    {
        ActorInstance _actor;
        Future<Exit> _intercepted;
        Future<A> _receivedA;

        [When]
        public void An_exit_is_sent_to_an_actor_that_intercepts_exit()
        {
            _intercepted = new Future<Exit>();
            _receivedA = new Future<A>();

            _actor = AnonymousActor.New(inbox =>
            {
                inbox.Receive<Exit>(message =>
                {
                    _intercepted.Complete(message);
                });

                inbox.Receive<A>(message =>
                {
                    _receivedA.Complete(message);
                });
            });
        }

        [Then]
        public void Should_prevent_the_actor_from_exiting()
        {
            AnonymousActor.New(inbox =>
                {
                    _actor.Request<Exit>(inbox)
                        .Within(5.Seconds())
                        .Receive<Response<Exit>>(x => { });
                });
            _intercepted.WaitUntilCompleted(500.Seconds()).ShouldBeTrue("Exit was not intercepted");

            _actor.Send(new A());
            _receivedA.WaitUntilCompleted(5.Seconds()).ShouldBeTrue("A was not handled, did actor exit?");
        }

        class A
        {
        }
    }

    [Scenario]
    public class When_an_exit_is_sent_to_an_actor
    {
        ActorInstance _actor;
        Future<A> _receivedA;

        [When]
        public void An_exit_is_sent_to_an_actor()
        {
            _receivedA = new Future<A>();

            _actor = AnonymousActor.New(inbox =>
            {
                inbox.Receive<A>(message =>
                {
                    _receivedA.Complete(message);
                });
            });
        }

        [Then]
        public void Should_prevent_subsequent_messages_from_activating()
        {
            _actor.Exit();
            _actor.Send(new A());

            var completed = _receivedA.WaitUntilCompleted(5.Seconds());

            completed.ShouldBeFalse("A should not have been handled, actor should have exited");
        }

        class A
        {
        }
    }
}