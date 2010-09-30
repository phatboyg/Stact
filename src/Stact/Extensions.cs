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
	using System.Linq.Expressions;
	using System.Reflection;
	using Actors;
	using Actors.Internal;
	using Channels;
	using Channels.Internal;
	using Fibers;
	using Magnum.Extensions;
	using Magnum.Reflection;


	public static class Extensions
	{
		/// <summary>
		/// Wraps the message in a request and sends it to the channel
		/// </summary>
		/// <typeparam name="TRequest">The type of the request message</typeparam>
		/// <param name="channel">The channel where the message should be sent</param>
		/// <param name="request">The request message</param>
		/// <param name="responseChannel">The channel where responses should be sent</param>
		public static void Request<TRequest>(this UntypedChannel channel, TRequest request, UntypedChannel responseChannel)
		{
			var requestImpl = new RequestImpl<TRequest>(responseChannel, request);

			channel.Send<Request<TRequest>>(requestImpl);
		}


		/// <summary>
		/// Wraps the message in a request and sends it to the channel
		/// </summary>
		/// <typeparam name="TRequest">The type of the request message</typeparam>
		/// <param name="channel">The channel where the message should be sent</param>
		/// <param name="request">The request message</param>
		/// <param name="inbox">The response inbox</param>
		public static SentRequest<TRequest> Request<TRequest>(this UntypedChannel channel, TRequest request, Inbox inbox)
		{
			UntypedChannel responseChannel = inbox;

			channel.Request(request, responseChannel);

			return new SentRequestImpl<TRequest>(request, inbox);
		}

		/// <summary>
		/// Wraps a message in a response and sends it to the response channel of the request
		/// </summary>
		/// <typeparam name="TRequest">The type of the request message</typeparam>
		/// <typeparam name="TResponse">The type of the response message</typeparam>
		/// <param name="request">The request context</param>
		/// <param name="response">The response message</param>
		public static void Respond<TRequest, TResponse>(this Request<TRequest> request, TResponse response)
		{
			request.ResponseChannel.Send<Response<TRequest, TResponse>>(new ResponseImpl<TRequest, TResponse>(response));
		}


		public static Uri ToMessageUrn(this Type type)
		{
			return new Uri("urn:message:" + type.FullName.Replace(".", ":"));
		}

		/// <summary>
		/// Sends an Exit message to an actor instance without waiting for a response
		/// </summary>
		/// <param name="instance">The actor instance</param>
		public static void Exit(this ActorInstance instance)
		{
			instance.Send<Exit>(new ExitImpl());
		}

		/// <summary>
		/// Sends an Exit message to an actor instance
		/// </summary>
		/// <param name="instance">The actor instance</param>
		/// <param name="sender">The exit request sender</param>
		public static SentRequest<Exit> Exit(this ActorInstance instance, Inbox sender)
		{
			return instance.Request<Exit>(new ExitImpl(), sender);
		}

		/// <summary>
		/// Sends a Kill message to an actor instance
		/// </summary>
		/// <param name="instance">The actor instance</param>
		public static void Kill(this ActorInstance instance)
		{
			instance.Send<Kill>(new KillImpl());
		}

		public static WithinSentRequest<TRequest> Within<TRequest>(this SentRequest<TRequest> request, TimeSpan timeout)
		{
			return new WithinSentRequestImpl<TRequest>(request, timeout);
		}



		public static void Connect<TActor, TPort>(this TActor actor, Expression<Func<TActor, Port<TPort>>> portProperty, Fiber fiber, Consumer<TPort> consumer)
			where TActor : Actor
		{
			PropertyInfo propertyInfo = portProperty.GetMemberPropertyInfo();
			var property = new FastProperty<TActor, Port<TPort>>(propertyInfo);

			var channel = new ConsumerChannel<TPort>(fiber, consumer);
			var port = new PortImpl<TPort>(channel);

			property.Set(actor, port);
		}
	}
}