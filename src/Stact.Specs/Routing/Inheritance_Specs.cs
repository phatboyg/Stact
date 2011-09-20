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
    using Magnum.TestFramework;
    using NUnit.Framework;
    using Routing;
    using Routing.Visualizers;


    [Scenario]
    public class When_an_inheritance_chain_is_part_of_a_message
    {
        DynamicRoutingEngine _engine;

        [When]
        public void An_inheritance_chain_is_part_of_a_message()
        {
            _engine = new DynamicRoutingEngine(new SynchronousFiber());

            _engine.Send(new OverTheTop());
        }

        [Then]
        [Explicit]
        public void Should_display_the_engine_routes()
        {
            new TraceRoutingEngineVisualizer().Show(_engine);
        }


        interface IBottom
        {
        }


        class Bottom :
            IBottom
        {
        }


        interface ITop
        {
        }


        class Top :
            Bottom, 
            ITop
        {
        }


        class OverTheTop :
            Top
        {
        }
    }
}