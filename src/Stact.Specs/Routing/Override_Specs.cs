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
    using Headers;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MessageHeaders;
    using NUnit.Framework;
    using Routing;


    [Scenario]
    public class When_two_receives_are_posted_for_a_message
    {
        MessageRoutingEngine _engine;
        Future<IBottom> _first;
        Future<IBottom> _second;

        [When]
        public void Two_receives_are_posted_for_a_message()
        {
            _first = new Future<IBottom>();
            _second = new Future<IBottom>();

            _engine = new MessageRoutingEngine();
            _engine.Configure(x =>
                {
                    x.Receive<IBottom>(_first.Complete);
                    x.Receive<IBottom>(_second.Complete);
                });

            _engine.Send(new MessageContext<OverTheTop>(new OverTheTop()));
        }

        [Then]
        public void Should_deliver_the_message_to_the_second_receive()
        {
            _second.WaitUntilCompleted(5.Seconds()).ShouldBeTrue();
        }

        [Then]
        public void Should_not_deliver_the_message_to_the_first_receive()
        {
            _first.WaitUntilCompleted(5.Seconds()).ShouldBeFalse();
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