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
namespace Stact
{
    using System.Collections.Generic;


    public interface MessageQueue
    {
        /// <summary>
        /// Enqueue the specified message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        void Enqueue<T>(Message<T> message);

        /// <summary>
        /// Dequeues all.
        /// </summary>
        /// <returns>The all.</returns>
        IList<Message> DequeueAll();

        /// <summary>
        /// Pushes a range from a list of messages to the front of the queue. Use case
        /// is that only n messages from the list were delivered and the remaining messages
        /// need to be requeued at the front for future delivery.
        /// </summary>
        /// <param name="messages">Messages.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="count">Count.</param>
        void PushFront(IList<Message> messages, int offset, int count);
    }
}