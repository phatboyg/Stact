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
namespace Stact.Specs.Redesign
{
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MessageHeaders;
    using Routing;
    using Routing.Internal;
    using Routing.Nodes;


    [Scenario]
    public class Inspecting_a_routing_engine
    {
        [Then]
        public void Should_navigate_properly()
        {
            RoutingEngine engine = new MessageRoutingEngine();

            var foundA = new Future<AlphaNode<Message<A>>>();
            var foundJoin = new Future<JoinNode<Message<A>>>();

            engine.Configure(x =>
                {
                    new MatchAlphaNode<Message<A>>(engine, alphaNode =>
                        {
                            foundA.Complete(alphaNode);

                            new MatchJoinNode<Message<A>>(alphaNode, joinNode => { foundJoin.Complete(joinNode); });
                        });
                });

            foundA.WaitUntilCompleted(5.Seconds()).ShouldBeTrue("Message A alpha node not found");
            foundJoin.WaitUntilCompleted(5.Seconds()).ShouldBeTrue("Message A constant join node not found");

            // new RoutingEngineTextVisualizer().Visit(engine);
        }

        [Then]
        public void Should_match_a_join_node()
        {
            RoutingEngine engine = new MessageRoutingEngine();

            var called = new Future<A>();

            engine.Configure(x => { x.Receive<A>(called.Complete); });

            engine.DispatchMessage(new MessageContext<A>(new A()));

            called.WaitUntilCompleted(5.Seconds()).ShouldBeTrue("Message was not delivered");

            // new RoutingEngineTextVisualizer().Visit(engine);
        }


        class A
        {
        }
    }
}