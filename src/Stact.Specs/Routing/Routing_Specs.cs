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
    using MessageHeaders;
    using NUnit.Framework;
    using Routing;


    [Scenario]
    public class When_routing_messages_using_the_routing_engine
    {
        RoutingEngine _engine;
        Future<A> _receivedA;
        Future<B> _receivedB;
        Future<C> _receivedC;
        Future<Message<B>> _receivedMessageB;
        Future<Message<C>> _receivedMessageC;

        [When]
        public void Should_properly_invoke_the_message_receiver()
        {
            _engine = new MessageRoutingEngine();

            _receivedA = new Future<A>();
            _receivedB = new Future<B>();
            _receivedC = new Future<C>();
            _receivedMessageB = new Future<Message<B>>();
            _receivedMessageC = new Future<Message<C>>();

            _engine.Configure(x =>
                {
                    x.Receive<A>(message => _receivedA.Complete(message.Body));
                    x.Receive<B>(message => _receivedB.Complete(message.Body));
                    x.Receive<C>(message => _receivedC.Complete(message.Body));
                    x.Receive<B>(_receivedMessageB.Complete);
                    x.Receive<C>(_receivedMessageC.Complete);
                });

            _engine.Send(new MessageContext<B>(new B()));
            _engine.Send(new MessageContext<C>(new C()));
            _engine.Send(new MessageContext<B>(new B()));
            _engine.Send(new MessageContext<C>(new C()));
        }

        [Then]
        [Explicit]
        public void Display_graph()
        {
//            RoutingEngineDebugVisualizer.Show(_engine);
        }

        [Then]
        public void Should_not_receive_an_a()
        {
            _receivedA.IsCompleted.ShouldBeFalse();
        }

        [Then]
        public void Should_receive_a_b()
        {
            _receivedB.IsCompleted.ShouldBeTrue();
        }

        [Then]
        public void Should_receive_a_c()
        {
            _receivedC.IsCompleted.ShouldBeTrue();
        }

        [Then]
        public void Should_receive_a_message_b()
        {
            _receivedMessageB.IsCompleted.ShouldBeTrue();
        }

        [Then]
        public void Should_receive_a_message_c()
        {
            _receivedMessageC.IsCompleted.ShouldBeTrue();
        }


        class A
        {
        }


        class B :
            A
        {
        }


        class C :
            B
        {
        }
    }
}