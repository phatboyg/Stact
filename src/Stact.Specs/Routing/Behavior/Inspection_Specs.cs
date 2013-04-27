// Copyright 2010-2013 Chris Patterson
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
namespace Stact.Specs.Redesign
{
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MessageHeaders;
    using Routing;
    using Routing.Configuration;
    using Routing.Nodes;


    [Scenario]
    public class Inspecting_a_routing_engine
    {
        [Then]
        public void Should_navigate_properly()
        {
            RoutingEngine engine = new MessageRoutingEngine();

            var foundA = new Future<AlphaNode<Message<A>>>();

            engine.Configure(x => { var node = new MatchAlphaNode<Message<A>>(engine, foundA.Complete); });

            foundA.WaitUntilCompleted(5.Seconds()).ShouldBeTrue("Message A alpha node not found");
        }

        [Then]
        public void Should_match_a_join_node()
        {
            RoutingEngine engine = new MessageRoutingEngine();

            var called = new Future<A>();

            engine.Configure(x => { x.Receive<A>(called.Complete); });

            engine.DispatchMessage(new MessageContext<A>(new A()));

            called.WaitUntilCompleted(5.Seconds()).ShouldBeTrue("Message was not delivered");
        }


        class A
        {
        }
    }
}