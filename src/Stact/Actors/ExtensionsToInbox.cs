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
namespace Stact.Actors
{
	using System;
	using Channels;
	using Internal;


	public static class ExtensionsToInbox
	{
		/// <summary>
		/// Wraps the message in a request and sends it to the channel
		/// </summary>
		/// <typeparam name="TRequest">The type of the request message</typeparam>
		/// <param name="channel">The channel where the message should be sent</param>
		/// <param name="request">The request message</param>
		/// <param name="responseChannel">The channel where responses should be sent</param>
		public static SentRequest<TRequest> Request<TRequest>(this UntypedChannel channel, TRequest request, Inbox inbox)
		{
			UntypedChannel responseChannel = inbox;

			channel.Request(request, responseChannel);

			return new SentRequestImpl<TRequest>(request, inbox);
		}

		public static WithinSentRequest<TRequest> Within<TRequest>(this SentRequest<TRequest> request, TimeSpan timeout)
		{
			return new WithinSentRequestImpl<TRequest>(request, timeout);
		}
	}
}