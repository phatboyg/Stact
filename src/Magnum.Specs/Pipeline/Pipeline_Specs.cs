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
    using System.Threading;
    using Magnum.Pipeline;
    using Magnum.Pipeline.Segments;
    using Magnum.Pipeline.Visitors;
    using NUnit.Framework;

    [TestFixture]
    public class Subscribing_to_the_pipe
    {
        [Test]
        public void Should_result_in_an_expression_being_called()
        {
            var called = new ManualResetEvent(false);

            var consumer = PipeSegment.Consumer<ClaimModified>(message => { called.Set(); });

            var recipients = new[] {consumer};

            var recipientList = PipeSegment.RecipientList<ClaimModified>(recipients);

            new TracePipeVisitor().Trace(recipientList);

            recipientList.Send(new ClaimModified());

            Assert.IsTrue(called.WaitOne(TimeSpan.Zero, false));
        }
    }

    [TestFixture]
    public class Using_a_subscription_context
    {
        private Pipe _pipe;
        private ManualResetEvent _received;

        [SetUp]
        public void Setup()
        {
            _received = new ManualResetEvent(false);

            var recipients = new Pipe[] { };
            var recipientList = PipeSegment.RecipientList<object>(recipients);
            _pipe = PipeSegment.Input(recipientList);
        }

        [Test]
        public void Should_subscribe_a_message_consumer_to_the_pipe()
        {
            using (var scope = _pipe.NewSubscriptionScope())
            {
                scope.Subscribe<ClaimModified>(message => { _received.Set(); });

                _pipe.Send(new ClaimModified());
            }

            Assert.IsTrue(_received.WaitOne(TimeSpan.Zero, false));
        }

        [Test]
        public void Should_unsubscribe_a_message_consumer_from_the_pipe()
        {
            using (var scope = _pipe.NewSubscriptionScope())
            {
                scope.Subscribe<ClaimModified>(message => { _received.Set(); });
            }

            _pipe.Send(new ClaimModified());

            Assert.IsFalse(_received.WaitOne(TimeSpan.Zero, false));
        }
    }

    public class ClaimModified :
        IDomainEvent
    {
        public string Text { get; set; }
    }

    public interface IDomainEvent
    {
    }
}