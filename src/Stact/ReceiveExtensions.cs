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
namespace Stact
{
    using System;


    public static class ReceiveExtensions
    {
        /// <summary>
        ///   Calls the specified method when a message of the requested type is received. The
        ///   consumer is asked if the message should be parsed, and returns a non-null action
        ///   if the message should be passed to the consumer. At that point, the message is removed
        ///   from the mailbox and delivered to the consumer
        /// </summary>
        /// <typeparam name = "T">The requested message type</typeparam>
        /// <param name="actor"></param>
        /// <param name = "consumer">The consumer</param>
        public static PendingReceive Receive<T>(this ActorInbox actor, Consumer<Message<T>> consumer)
        {
            return actor.Receive<T>(x => consumer);
        }

        /// <summary>
        ///   Calls the specified method when a message of the requested type is received. The
        ///   consumer is asked if the message should be parsed, and returns a non-null action
        ///   if the message should be passed to the consumer. At that point, the message is removed
        ///   from the mailbox and delivered to the consumer
        /// </summary>
        /// <typeparam name = "T">The requested message type</typeparam>
        /// <param name="inbox">The inbox to receive the message from</param>
        /// <param name = "consumer">The consumer</param>
        /// <param name = "timeout">The time period to wait for a message</param>
        /// <param name = "timeoutCallback">The method to call if a message is not received within the timeout period</param>
        public static PendingReceive Receive<T>(this ActorInbox inbox, Consumer<Message<T>> consumer, TimeSpan timeout,
                                                Action timeoutCallback)
        {
            return inbox.Receive<T>(x => consumer, timeout, timeoutCallback);
        }

        /// <summary>
        ///   Calls the specified method when a message of the requested type is received. The
        ///   consumer is asked if the message should be parsed, and returns a non-null action
        ///   if the message should be passed to the consumer. At that point, the message is removed
        ///   from the mailbox and delivered to the consumer
        /// </summary>
        /// <typeparam name = "T">The requested message type</typeparam>
        /// <param name="inbox">The inbox to receive the message from</param>
        /// <param name = "consumer">The consumer</param>
        /// <param name = "timeout">The time period to wait for a message</param>
        /// <param name = "timeoutCallback">The method to call if a message is not received within the timeout period</param>
        public static PendingReceive Receive<T>(this ActorInbox inbox, Consumer<Message<T>> consumer, int timeout,
                                                Action timeoutCallback)
        {
            return inbox.Receive<T>(x => consumer, TimeSpan.FromMilliseconds(timeout), timeoutCallback);
        }

        /// <summary>
        ///   Calls the specified method when a message of the requested type is received. The
        ///   consumer is asked if the message should be parsed, and returns a non-null action
        ///   if the message should be passed to the consumer. At that point, the message is removed
        ///   from the mailbox and delivered to the consumer
        /// </summary>
        /// <typeparam name = "T">The requested message type</typeparam>
        /// <param name="inbox">The inbox to receive the message from</param>
        /// <param name = "consumer">The consumer</param>
        /// <param name = "timeout">The time period to wait for a message</param>
        /// <param name = "timeoutCallback">The method to call if a message is not received within the timeout period</param>
        public static PendingReceive Receive<T>(this ActorInbox inbox, SelectiveConsumer<Message<T>> consumer,
                                                int timeout, Action timeoutCallback)
        {
            return inbox.Receive(consumer, TimeSpan.FromMilliseconds(timeout), timeoutCallback);
        }
    }
}