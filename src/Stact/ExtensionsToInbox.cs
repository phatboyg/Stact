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
	using System.Diagnostics;
	using Internal;
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

		public static void Loop(this Inbox inbox, Action<ReceiveLoop> loopAction)
		{
			var loop = new ReceiveLoopImpl(inbox);

			loopAction(loop);

			loop.Repeat();
		}

		public static ReceiveLoop EnableSuspendResume(this ReceiveLoop loop, Inbox inbox)
		{
			return loop.Receive<Suspend>(pause =>
				{
					// we are going to only receive a continue until we get it
					inbox.Receive<Resume>(x =>
						{
							// repeat the loop now
							loop.Repeat();
						});
				});
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
		///   Wraps the message in a request and sends it to the channel
		/// </summary>
		/// <typeparam name = "TRequest">The type of the request message</typeparam>
		/// <param name = "channel">The channel where the message should be sent</param>
		/// <param name = "request">The request message</param>
		/// <param name = "inbox">The response inbox</param>
		public static SentRequest<TRequest> Request<TRequest>(this UntypedChannel channel, TRequest request, Inbox inbox)
		{
			UntypedChannel responseChannel = inbox;

			Request<TRequest> sent = channel.Request(request, responseChannel);

			return new SentRequestImpl<TRequest>(sent, inbox);
		}

		public static SentRequest<TRequest> Request<TRequest>(this UntypedChannel channel, Inbox inbox)
		{
			UntypedChannel responseChannel = inbox;

			Request<TRequest> sent = channel.Request<TRequest>(responseChannel);

			return new SentRequestImpl<TRequest>(sent, inbox);
		}


		public static WithinSentRequest<TRequest> Within<TRequest>(this SentRequest<TRequest> request, TimeSpan timeout)
		{
			return new WithinSentRequestImpl<TRequest>(request, timeout);
		}
	}
}