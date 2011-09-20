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
    using System;
    using MessageHeaders;
    using NUnit.Framework;
    using Routing.Contexts;
    using Routing.Internal;


    [TestFixture]
    public class Using_the_context_factory
    {
        [Test]
        public void Should_identify_a_wrapped_message()
        {
            var message = new MyMessage();
            Message<MyMessage> wrapped = new MessageImpl<MyMessage>(message);

            var collector = new TestCollector(typeof(MyMessage));

            _contextFactory.Create(wrapped, collector);
        }

        [Test]
        public void Should_identify_a_wrapped_message_behind_the_interface()
        {
            var message = new MyMessage();
            var wrapped = new MessageImpl<MyMessage>(message);

            var collector = new TestCollector(typeof(MyMessage));

            _contextFactory.Create(wrapped, collector);
        }

        [Test]
        public void Should_properly_wrap_a_raw_message()
        {
            var message = new MyMessage();

            var collector = new TestCollector(typeof(MyMessage));

            _contextFactory.Create(message, collector);
        }

        RoutingContextFactory _contextFactory;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _contextFactory = new DynamicRoutingContextFactory();
        }


        class TestCollector :
            Activation
        {
            Type _bodyType;

            public TestCollector(Type bodyType)
            {
                _bodyType = bodyType;
            }

            public void Activate<T>(RoutingContext<T> message)
            {
                Assert.AreEqual(_bodyType, typeof(T), "The type of body was not as expected");
            }
        }


        class MyMessage
        {
        }
    }
}