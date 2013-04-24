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
namespace Stact.Specs
{
    using Magnum.TestFramework;
    using MessageHeaders;
    using NUnit.Framework;
    using Routing;


    [Scenario]
    public class When_an_inheritance_chain_is_part_of_a_message
    {
        MessageRoutingEngine _engine;

        [When]
        public void An_inheritance_chain_is_part_of_a_message()
        {
            _engine = new MessageRoutingEngine();

            _engine.Send(new MessageContext<OverTheTop>(new OverTheTop()));
        }

        [Then]
        [Explicit]
        public void Should_display_the_engine_routes()
        {
//            new TraceRoutingEngineVisualizer().Show(_engine);
        }


        class Bottom :
            IBottom
        {
        }


        interface IBottom
        {
        }


        interface ITop
        {
        }


        class OverTheTop :
            Top
        {
        }


        class Top :
            Bottom,
            ITop
        {
        }
    }
}