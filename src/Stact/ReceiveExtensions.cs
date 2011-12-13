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
        public static ReceiveHandle Receive<T>(this UntypedActor actor, Consumer<Message<T>> consumer)
        {
            return actor.Receive<T>(x => consumer);
        }
    }
}