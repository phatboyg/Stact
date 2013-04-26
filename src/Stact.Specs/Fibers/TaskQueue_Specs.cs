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
    using System;
    using System.Collections.Generic;
    using MessageHeaders;
    using NUnit.Framework;


    public class TraceMessageDispatcher :
        List<string>,
        MessageDispatcher
    {
        void MessageDispatcher.DispatchMessage<T>(Message<T> message)
        {
            Add(typeof(T).Name);
        }
    }


    [TestFixture]
    public class TaskQueue_Specs
    {
        [Test]
        public void Should_be_able_to_dispatch_messages()
        {
            MessageQueue queue = new ListMessageQueue();
            queue.Enqueue(new MessageContext<string>("Hello"));
            queue.Enqueue(new MessageContext<string>("World"));


            var dispatcher = new TraceMessageDispatcher();
            var executor = new MessageQueueExecution(queue, dispatcher);

            var fiber = new TaskFiber();
            fiber.Add(executor);

            Assert.IsTrue(fiber.Stop(TimeSpan.FromSeconds(10000)), "Failed to shutdown fiber");

            Assert.AreEqual(2, dispatcher.Count, "Expected two strings");
            Assert.AreEqual("String", dispatcher[0]);
            Assert.AreEqual("String", dispatcher[1]);
        }
    }


    class Mailbox
    {
        readonly MessageQueue _queue;
        readonly Fiber _fiber;
        readonly MessageDispatcher _dispatcher;

        public Mailbox()
        {
            _queue = new ListMessageQueue();
            _fiber = new TaskFiber();
        }

        public void Send<T>(T body)
        {
            var message = new MessageContext<T>(body);

            _queue.Enqueue(message);

            _fiber.Add(new MessageQueueExecution(_queue, _dispatcher));
        }
    }
}