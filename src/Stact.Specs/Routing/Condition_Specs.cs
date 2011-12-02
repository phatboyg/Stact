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
namespace Stact.Specs
{
    using Magnum.Extensions;
    using Magnum.TestFramework;


    [Scenario]
    public class Condition_Specs
    {
        [Then]
        public void Should_properly_invoke_the_message_receiver()
        {
            var received1 = new Future<Message<A>>();
            var received2 = new Future<Message<A>>();

            Message<A> request1;
            Message<A> request2;

            Actor.New(this, actor =>
                {
                    request1 = actor.Self.Request(new A(), actor);
                    request2 = actor.Self.Request(new A(), actor);

                    actor.Receive<A>(x => x.RequestId != request2.RequestId
                                             ? (Consumer<Message<A>>)null
                                             : received2.Complete);
                    actor.Receive<A>(x => x.RequestId != request1.RequestId
                                             ? (Consumer<Message<A>>)null
                                             : received1.Complete);
                });


            received1.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("1 not received");
            received2.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("2 not received");
        }


        class A
        {
        }


        class MyActor
        {
        }
    }
}