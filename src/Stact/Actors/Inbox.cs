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
	


	/// <summary>
	///   Mailbox is a higher level construct than a channel, providing channel aggregation,
	///   directed receives, and dispatching to channels within a context, such as an actor
	/// </summary>
	public interface Inbox :
		UntypedChannel
	{
		/// <summary>
		///   Calls the specified method when a message of the requested type is received. The
		///   consumer is asked if the message should be parsed, and returns a non-null action
		///   if the message should be passed to the consumer. At that point, the message is removed
		///   from the mailbox and delivered to the consumer
		/// </summary>
		/// <typeparam name = "T">The requested message type</typeparam>
		/// <param name = "consumer">The consumer</param>
		PendingReceive Receive<T>(SelectiveConsumer<T> consumer);

		/// <summary>
		///   Specifies a method to call when a message is recieved. If a message is not received within
		///   the specified timeout, a timeout callback is called instead and the receiver is removed from 
		///   the waiting list of receivers.
		/// </summary>
		/// <param name = "consumer">The consumer to call with the message</param>
		/// <param name = "timeout">The time period to wait for a message</param>
		/// <param name = "timeoutCallback">The method to call if a message is not received within the timeout period</param>
		PendingReceive Receive<T>(SelectiveConsumer<T> consumer, TimeSpan timeout, Action timeoutCallback);
	}
}