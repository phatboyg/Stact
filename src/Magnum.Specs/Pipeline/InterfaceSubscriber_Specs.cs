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
    using Consumers;
    using Magnum.Pipeline;
    using Magnum.Pipeline.Segments;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class Given_an_established_pipe
    {
        protected Pipe Input { get; set; }
        protected ISubscriptionScope Scope { get; private set; }

        [SetUp]
        public void Setup()
        {
            Input = PipeSegment.Input(PipeSegment.End());
            Scope = Input.NewSubscriptionScope();

            EstablishContext();
        }

        protected virtual void EstablishContext()
        {
        }

        [TearDown]
        public void Teardown()
        {
            ExitContext();

            Scope.Dispose();
            Input = null;
        }

        protected virtual void ExitContext()
        {
        }
    }

    [TestFixture]
    public class Subscribing_a_consumer_with_a_single_message_consumer :
        Given_an_established_pipe
    {
        protected SingleMessageConsumer Consumer { get; private set; }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            Consumer = new SingleMessageConsumer();
            Scope.Subscribe(Consumer);
        }

        [Test]
        public void Should_deliver_messages_to_the_consumer()
        {
            Input.Send(new ClaimModified());

            Assert.IsTrue(Consumer.ClaimModifiedCalled.IsCompleted);
        }
    }

    [TestFixture]
    public class Subscribing_a_consumer_with_multiple_message_consumers :
        Given_an_established_pipe
    {
        protected MultipleMessageConsumer Consumer { get; private set; }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            Consumer = new MultipleMessageConsumer();
            Scope.Subscribe(Consumer);
        }

        [Test]
        public void Should_deliver_messages_to_the_first_consumer()
        {
            Input.Send(new ClaimModified());

            Assert.IsTrue(Consumer.ClaimModifiedCalled.IsCompleted);
        }

        [Test]
        public void Should_deliver_messages_to_the_second_consumer()
        {
            Input.Send(new ClaimCreated());

            Assert.IsTrue(Consumer.ClaimCreatedCalled.IsCompleted);
        }
    }
}