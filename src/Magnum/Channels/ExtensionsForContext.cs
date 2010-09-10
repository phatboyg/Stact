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
namespace Magnum.Channels
{
	using System;
	using Context;


	public static class ExtensionsForContext
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
			channel.Send<Request<TRequest>>(new RequestImpl<TRequest>(responseChannel, request));
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
			request.ResponseChannel.Send<Response<TRequest,TResponse>>(new ResponseImpl<TRequest,TResponse>(response));
		}

		public static Uri ToMessageUrn(this Type type)
		{
			return new Uri("urn:message:" + type.FullName.Replace(".", ":"));
		}
	}
}