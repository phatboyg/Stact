// Copyright 2007-2010 The Apache Software Foundation.
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
	using Actors;
	using Channels;
	using Magnum.Extensions;


	public static class ExtensionsToInbox
	{
		/// <summary>
		///   Calls the specified method when a message of the requested type is received. The
		///   consumer is asked if the message should be parsed, and returns a non-null action
		///   if the message should be passed to the consumer. At that point, the message is removed
		///   from the mailbox and delivered to the consumer
		/// </summary>
		/// <typeparam name = "T">The requested message type</typeparam>
		/// <param name="inbox">The inbox to receive the message from</param>
		/// <param name = "consumer">The consumer</param>
		public static PendingReceive Receive<T>(this Inbox inbox, Consumer<T> consumer)
		{
			return inbox.Receive<T>(x => consumer);
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
		public static PendingReceive Receive<T>(this Inbox inbox, Consumer<T> consumer, TimeSpan timeout,
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
		public static PendingReceive Receive<T>(this Inbox inbox, Consumer<T> consumer, int timeout, Action timeoutCallback)
		{
			return inbox.Receive<T>(x => consumer, timeout.Milliseconds(), timeoutCallback);
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
		public static PendingReceive Receive<T>(this Inbox inbox, SelectiveConsumer<T> consumer, int timeout,
		                                        Action timeoutCallback)
		{
			return inbox.Receive(consumer, timeout.Milliseconds(), timeoutCallback);
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
		public static PendingReceive Receive<T>(this Inbox<T> inbox, Consumer<T> consumer, int timeout, Action timeoutCallback)
		{
			return inbox.Receive(x => consumer, timeout.Milliseconds(), timeoutCallback);
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
		public static PendingReceive Receive<T>(this Inbox<T> inbox, SelectiveConsumer<T> consumer, int timeout,
		                                        Action timeoutCallback)
		{
			return inbox.Receive(consumer, timeout.Milliseconds(), timeoutCallback);
		}
	}
}