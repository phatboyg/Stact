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
namespace Stact.Specs.Behavior
{
    using Magnum.TestFramework;
    using Routing;
    using Routing.Internal;
    using Routing.Nodes;


    [Scenario]
    public class When_adding_a_request_message
    {
        MessageRoutingEngine _engine;

        [When]
        public void Adding_a_request_message()
        {
            _engine = new MessageRoutingEngine();

            //_engine.Request(new A(), new ShuntChannel());
        }

        [Then]
        public void Should_result_in_two_paths_through_the_routing_engine()
        {
            var foundA = new Future<AlphaNode<Message<A>>>();
            var foundRequestA = new Future<AlphaNode<Message<A>>>();

            new MatchAlphaNode<Message<A>>(_engine, foundA.Complete);
            new MatchAlphaNode<Message<A>>(_engine, foundRequestA.Complete);

            foundRequestA.IsCompleted.ShouldBeTrue("Could not find request alpha node");
            foundA.IsCompleted.ShouldBeTrue("Could not find body alpha node");
        }


        class A
        {
        }
    }
}