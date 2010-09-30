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
namespace Stact.Specs.Pipeline
{
    using System;
    using System.Threading;
    using Stact.Pipeline;
    using Stact.Pipeline.Segments;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class Given_an_existing_pipe_with_subscriptions
    {
        protected Pipe _pipe;
        protected ManualResetEvent _called;
        private ISubscriptionScope _subscriptionScope;

        [SetUp]
        public void Setup()
        {
            _pipe = PipeSegment.Input(PipeSegment.End<object>());
            _subscriptionScope = _pipe.NewSubscriptionScope();

            _called = new ManualResetEvent(false);
            _subscriptionScope.Subscribe<ClaimModified>(x => _called.Set());

            EstablishContext();
        }

        [TearDown]
        public void Teardown()
        {
            _subscriptionScope.Dispose();
            _subscriptionScope = null;
        }

        protected virtual void EstablishContext()
        {
        }
    }

    [TestFixture]
    public class Adding_an_interceptor :
        Given_an_existing_pipe_with_subscriptions
    {
        protected override void EstablishContext()
        {
            using (var scope = _pipe.NewSubscriptionScope())
            {
                scope.Intercept<object>(x => { });

                _pipe.Send(new ClaimModified());
            }
        }

        [Test]
        public void Should_still_result_in_the_consumer_being_called()
        {
            Assert.IsTrue(_called.WaitOne(TimeSpan.Zero), "The message handler was not called");
        }
    }
}