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


    public interface UntypedActor
    {
        ActorRef Self { get; }

        /// <summary>
        /// Sends a message to the actor inbox
        /// </summary>
        /// <typeparam name="T">The type of message sent</typeparam>
        /// <param name="message">The message to send</param>
        void Send<T>(Message<T> message);

        /// <summary>
        /// Add a receive to the inbox for the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="consumer">The message consumer</param>
        /// <returns>A handle to the pending receive call</returns>
        ReceiveHandle Receive<T>(SelectiveConsumer<Message<T>> consumer);

        /// <summary>
        /// Sets a timeout, after which time the specified callback will be invoked.
        /// </summary>
        /// <param name="timeout">The timeout period</param>
        /// <param name="timeoutCallback">The callback to invoke if the timeout expires</param>
        /// <returns>A TimeoutHandle, which can be used to cancel the timeout callback</returns>
        TimeoutHandle SetTimeout(TimeSpan timeout, Action timeoutCallback);

        /// <summary>
        /// Sets the current exception handler for the actor, pushing previously specified exception
        /// handlers onto a stack.
        /// </summary>
        /// <param name="exceptionHandler">The next exception handler</param>
        void SetExceptionHandler(ActorExceptionHandler exceptionHandler);
    }
}