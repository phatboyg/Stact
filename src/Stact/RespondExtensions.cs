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


    public static class RespondExtensions
    {
        /// <summary>
        ///   Wraps a message in a response and sends it to the response channel of the request
        /// </summary>
        /// <typeparam name = "TRequest">The type of the request message</typeparam>
        /// <typeparam name = "TResponse">The type of the response message</typeparam>
        /// <param name = "request">The request context</param>
        /// <param name = "response">The response message</param>
        public static void Respond<TResponse>(this Message request, TResponse response)
        {
            request.Sender.Send(response, header => header.SetRequestId(request.RequestId));
        }

        public static void Respond<TResponse>(this Message request, TResponse response,
                                              Action<SetMessageHeader> headerCallback)
        {
            request.Sender.Send(response, header =>
                {
                    header.SetRequestId(request.RequestId);
                    headerCallback(header);
                });
        }

        public static void Respond<TResponse>(this Message request)
            where TResponse : class
        {
            if (!typeof(TResponse).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var response = DynamicProxyFactory.Get<TResponse>();

            request.Sender.Send(response, x => x.SetRequestId(request.RequestId));
        }

        public static void Respond<TResponse>(this Message request, object values)
            where TResponse : class
        {
            if (!typeof(TResponse).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var response = DynamicProxyFactory.Get<TResponse>();
  //          var response = InterfaceImplementationExtensions.InitializeProxy<TResponse>(values);

            request.Sender.Send(response, x => x.SetRequestId(request.RequestId));
        }

        public static void Respond<TResponse>(this Message request, object values,
                                              Action<SetMessageHeader> headerCallback)
            where TResponse : class
        {
            if (!typeof(TResponse).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var response = DynamicProxyFactory.Get<TResponse>();
//            var response = InterfaceImplementationExtensions.InitializeProxy<TResponse>(values);

            request.Sender.Send(response, header =>
                {
                    header.SetRequestId(request.RequestId);
                    headerCallback(header);
                });
        }
    }
}