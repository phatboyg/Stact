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
namespace Stact.Specs.Fibers
{
    using MessageHeaders;
    using NUnit.Framework;


    [TestFixture]
    public class Queue_Specs
    {
        [Test]
        public void Should_be_creatable()
        {
            MessageQueue queue = new ListMessageQueue();

            Assert.IsNotNull(queue);
        }

        [Test]
        public void Should_allow_messages_to_be_queued()
        {
            MessageQueue queue = new ListMessageQueue();
            queue.Enqueue(new MessageContext<string>("Hello"));

            var messages = queue.DequeueAll();

            Assert.AreEqual(1, messages.Count);
        }


        [Test]
        public void Should_allow_messages_to_be_pushed_to_the_front()
        {
            MessageQueue queue = new ListMessageQueue();
            queue.Enqueue(new MessageContext<string>("Hello"));
            queue.Enqueue(new MessageContext<string>("World"));

            var messages = queue.DequeueAll();

            queue.Enqueue(new MessageContext<int>(42));

            queue.PushFront(messages, 0, 2);

            var final = queue.DequeueAll();

            Assert.AreEqual(3, final.Count);

            Assert.IsInstanceOfType(typeof(Message<string>), final[0]);
            Assert.IsInstanceOfType(typeof(Message<string>), final[1]);
            Assert.IsInstanceOfType(typeof(Message<int>), final[2]);
        }

        [Test]
        public void Should_allow_some_messages_to_be_pushed_to_the_front()
        {
            MessageQueue queue = new ListMessageQueue();
            queue.Enqueue(new MessageContext<string>("Hello"));
            queue.Enqueue(new MessageContext<string>("World"));

            var messages = queue.DequeueAll();

            queue.Enqueue(new MessageContext<int>(42));

            queue.PushFront(messages, 1, 1);

            var final = queue.DequeueAll();

            Assert.AreEqual(2, final.Count);

            Assert.IsInstanceOfType(typeof(Message<string>), final[0]);
            Assert.IsInstanceOfType(typeof(Message<int>), final[1]);
        }

    }
}