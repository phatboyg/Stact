// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.Specs.Pipeline
{
    using System;
    using Magnum.Actors;
    using Magnum.Pipeline;
    using Magnum.Pipeline.Messages;
    using Magnum.Pipeline.Segments;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class Adding_a_subscription_to_the_pipeline
    {
        private InputSegment _input;
        private ISubscriptionScope _subscriberScope;
        private Future<bool> _addCalled;
        private Future<bool> _removeCalled;

        [SetUp]
        public void Setup()
        {
            _addCalled = new Future<bool>();
            _removeCalled = new Future<bool>();

            _input = PipeSegment.Input(PipeSegment.End());

            _subscriberScope = _input.NewSubscriptionScope();
            _subscriberScope.Subscribe<SubscriberAdded>(x => _addCalled.Complete(true));
            _subscriberScope.Subscribe<SubscriberRemoved>(x => _removeCalled.Complete(true));

            using (var scope = _input.NewSubscriptionScope())
            {
                scope.Subscribe<ClaimModified>(x => { });
            }
        }

        [TearDown]
        public void Teardown()
        {
            _subscriberScope.Dispose();
        }

        [Test]
        public void Should_call_the_subscription_added_handler()
        {
            Assert.IsTrue(_addCalled.IsAvailable());
        }

        [Test]
        public void Should_call_the_subscription_removed_handler()
        {
            Assert.IsTrue(_removeCalled.IsAvailable());
        }
    }
}