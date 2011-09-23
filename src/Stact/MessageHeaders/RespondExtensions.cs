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
    using Magnum.Reflection;
    using MessageHeaders;


    public static class RespondExtensions
    {
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

        public static void Respond<TRequest>(this Request<TRequest> request)
        {
            if (!typeof(TRequest).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            Type requestImplType = InterfaceImplementationBuilder.GetProxyFor(typeof(TRequest));

            var responseImpl = new ResponseImpl<TRequest, TRequest>(request, request.Body);

            request.ResponseChannel.Send<Response<TRequest, TRequest>>(responseImpl);
        }

        public static void Respond<TResponse>(this UntypedChannel channel, TResponse response, string requestId)
        {
            var responseImpl = new ResponseImpl<TResponse>(response, requestId);

            channel.Send<Response<TResponse>>(responseImpl);
        }
    }
}