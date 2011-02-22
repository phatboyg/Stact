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
	using Internal;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using MessageHeaders;


	public static class ExtensionsToHeaders
	{
		public static Message<T> Send<T>(this UntypedChannel channel,
			T message, Action<SetMessageHeader> messageCallback)
		{
			var messageImpl = new MessageImpl<T>(message);
			messageCallback(messageImpl);

			channel.Send<Message<T>>(messageImpl);

			return messageImpl;
		}

		public static Message<T> Send<T>(this UntypedChannel channel,
			Message<T> message, Action<SetMessageHeader> messageCallback)
		{
			var impl = message as MessageImpl<T>;
			if (impl != null)
				messageCallback(impl);

			channel.Send(message);

			return message;
		}

		/// <summary>
		///   Wraps the message in a request and sends it to the channel
		/// </summary>
		/// <typeparam name = "TRequest">The type of the request message</typeparam>
		/// <param name = "channel">The channel where the message should be sent</param>
		/// <param name = "request">The request message</param>
		/// <param name = "responseChannel">The channel where responses should be sent</param>
		public static Request<TRequest> Request<TRequest>(this UntypedChannel channel, 
			TRequest request, UntypedChannel responseChannel)
		{
			var requestImpl = new RequestImpl<TRequest>(responseChannel, request);

			channel.Send<Request<TRequest>>(requestImpl);

			return requestImpl;
		}

		/// <summary>
		///   Wraps the message in a request and sends it to the channel
		/// </summary>
		/// <typeparam name = "TRequest">The type of the request message</typeparam>
		/// <param name = "channel">The channel where the message should be sent</param>
		/// <param name = "request">The request message</param>
		/// <param name = "responseChannel">The channel where responses should be sent</param>
		public static Request<TRequest> Request<TRequest>(this Channel<Request<TRequest>> channel, 
			TRequest request, UntypedChannel responseChannel)
		{
			var requestImpl = new RequestImpl<TRequest>(responseChannel, request);

			channel.Send(requestImpl);

			return requestImpl;
		}

		/// <summary>
		///   Sends an uninitialized interface implementation as a request
		/// </summary>
		/// <typeparam name = "TRequest">The request message type, which must be an interface</typeparam>
		/// <param name = "channel">The target channel</param>
		/// <param name = "responseChannel">The channel where responses should be sent</param>
		public static Request<TRequest> Request<TRequest>(this UntypedChannel channel, 
			UntypedChannel responseChannel)
		{
			if (!typeof(TRequest).IsInterface)
				throw new ArgumentException("Default Implementations can only be created for interfaces");

			Type requestImplType = InterfaceImplementationBuilder.GetProxyFor(typeof(TRequest));

			var request = (TRequest)FastActivator.Create(requestImplType);

			var requestImpl = new RequestImpl<TRequest>(responseChannel, request);

			channel.Send<Request<TRequest>>(requestImpl);

			return requestImpl;
		}

		/// <summary>
		///   Sends an uninitialized interface implementation as a request
		/// </summary>
		/// <typeparam name = "TRequest">The request message type, which must be an interface</typeparam>
		/// <param name = "channel">The target channel</param>
		/// <param name = "responseChannel">The channel where responses should be sent</param>
		public static Request<TRequest> Request<TRequest>(this Channel<Request<TRequest>> channel,
			UntypedChannel responseChannel)
		{
			if (!typeof(TRequest).IsInterface)
				throw new ArgumentException("Default Implementations can only be created for interfaces");

			Type requestImplType = InterfaceImplementationBuilder.GetProxyFor(typeof(TRequest));

			var request = (TRequest)FastActivator.Create(requestImplType);

			var requestImpl = new RequestImpl<TRequest>(responseChannel, request);

			channel.Send(requestImpl);

			return requestImpl;
		}

		/// <summary>
		///   Sends an uninitialized interface implementation
		/// </summary>
		/// <typeparam name = "T">The request message type, which must be an interface</typeparam>
		/// <param name = "channel">The target channel</param>
		public static void Send<T>(this UntypedChannel channel)
		{
			if (!typeof(T).IsInterface)
				throw new ArgumentException("Default Implementations can only be created for interfaces");

			Type requestImplType = InterfaceImplementationBuilder.GetProxyFor(typeof(T));

			var message = (T)FastActivator.Create(requestImplType);

			channel.Send(message);
		}


		/// <summary>
		///   Wraps a message in a response and sends it to the response channel of the request
		/// </summary>
		/// <typeparam name = "TRequest">The type of the request message</typeparam>
		/// <typeparam name = "TResponse">The type of the response message</typeparam>
		/// <param name = "request">The request context</param>
		/// <param name = "response">The response message</param>
		public static void Respond<TRequest, TResponse>(this Request<TRequest> request, TResponse response)
		{
			var responseImpl = new ResponseImpl<TRequest, TResponse>(request, response);

			request.ResponseChannel.Send<Response<TRequest, TResponse>>(responseImpl);
		}

		public static void Respond<TResponse>(this UntypedChannel channel, TResponse response, string requestId)
		{
			var responseImpl = new ResponseImpl<TResponse>(response, requestId);

			channel.Send<Response<TResponse>>(responseImpl);
		}

		public static Uri GetUri(this Headers headers, string key)
		{
			string value = headers[key];
			if (string.IsNullOrEmpty(value))
				return null;

			return new Uri(value);
		}

		public static void SetUri(this Headers headers, string key, Uri value)
		{
			if (value == null)
			{
				headers[key] = null;
				return;
			}

			headers[key] = value.ToString();
		}
	}
}