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
    using Magnum.Reflection;
    using MessageHeaders;


    public static class RequestExtensions
    {
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

        public static Request<TRequest> Request<TRequest>(this UntypedChannel channel, object values,
                                                          UntypedChannel responseChannel)
            where TRequest : class
        {
            return Request<TRequest>(channel, values, x => { }, responseChannel);
        }

        public static Request<TRequest> Request<TRequest>(this UntypedChannel channel, object values,
                                                          Action<SetMessageHeader> messageCallback,
                                                          UntypedChannel responseChannel)
            where TRequest : class
        {
            if (!typeof(TRequest).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var request = InterfaceImplementationExtensions.InitializeProxy<TRequest>(values);

            var requestImpl = new RequestImpl<TRequest>(responseChannel, request);
            messageCallback(requestImpl);

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

        public static Request<TRequest> Request<TRequest>(this Channel<Request<TRequest>> channel, object values,
                                                          UntypedChannel responseChannel)
            where TRequest : class
        {
            if (!typeof(TRequest).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var request = InterfaceImplementationExtensions.InitializeProxy<TRequest>(values);

            var requestImpl = new RequestImpl<TRequest>(responseChannel, request);

            channel.Send(requestImpl);

            return requestImpl;
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

        public static SentRequest<TRequest> Request<TRequest>(this UntypedChannel channel, object values, Inbox inbox)
            where TRequest : class
        {
            UntypedChannel responseChannel = inbox;

            Request<TRequest> sent = channel.Request<TRequest>(values, responseChannel);

            return new SentRequestImpl<TRequest>(sent, inbox);
        }

        public static SentRequest<TRequest> Request<TRequest>(this UntypedChannel channel, object values,
                                                              Action<SetMessageHeader> messageCallback, Inbox inbox)
            where TRequest : class
        {
            UntypedChannel responseChannel = inbox;

            Request<TRequest> sent = channel.Request<TRequest>(values, messageCallback, responseChannel);

            return new SentRequestImpl<TRequest>(sent, inbox);
        }

        public static SentRequest<TRequest> Request<TRequest>(this UntypedChannel channel, Inbox inbox)
        {
            UntypedChannel responseChannel = inbox;

            Request<TRequest> sent = channel.Request<TRequest>(responseChannel);

            return new SentRequestImpl<TRequest>(sent, inbox);
        }
    }
}