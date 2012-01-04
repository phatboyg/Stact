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
    using Magnum;
    using Magnum.Reflection;
    using MessageHeaders;


    public static class RequestExtensions
    {
        /// <summary>
        ///   Wraps the message in a request and sends it to the channel
        /// </summary>
        /// <typeparam name = "TRequest">The type of the request message</typeparam>
        /// <param name = "actor">The channel where the message should be sent</param>
        /// <param name = "request">The request message</param>
        /// <param name = "sender">The channel where responses should be sent</param>
        public static Message<TRequest> Request<TRequest>(this ActorRef actor, TRequest request, ActorRef sender)
        {
            var context = new MessageContext<TRequest>(request, () => sender);

            return Send(actor, context);
        }

        /// <summary>
        ///   Sends an uninitialized interface implementation as a request
        /// </summary>
        /// <typeparam name = "TRequest">The request message type, which must be an interface</typeparam>
        /// <param name = "actor">The target channel</param>
        /// <param name="sender"></param>
        public static Message<TRequest> Request<TRequest>(this ActorRef actor, ActorRef sender)
        {
            if (!typeof(TRequest).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            Type requestType = InterfaceImplementationBuilder.GetProxyFor(typeof(TRequest));
            var request = (TRequest)FastActivator.Create(requestType);

            return Send(actor, new MessageContext<TRequest>(request, () => sender));
        }

        public static Message<TRequest> Request<TRequest>(this ActorRef actor, object values, ActorRef sender)
            where TRequest : class
        {
            return Request<TRequest>(actor, values, sender, x => { });
        }

        public static Message<TRequest> Request<TRequest>(this ActorRef actor, object values, ActorRef sender,
                                                          Action<SetMessageHeader> messageCallback)
            where TRequest : class
        {
            if (!typeof(TRequest).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var request = InterfaceImplementationExtensions.InitializeProxy<TRequest>(values);

            var context = new MessageContext<TRequest>(request, () => sender);
            messageCallback(context);

            return Send(actor, context, context.RequestId ?? CombGuid.Generate().ToString("N"));
        }

        static Message<T> Send<T>(ActorRef actor, MessageContext<T> context)
        {
            return Send(actor, context, CombGuid.Generate().ToString("N"));
        }

        static Message<T> Send<T>(ActorRef actor, MessageContext<T> context, string requestId)
        {
            context.SetRequestId(requestId);

            actor.Send(context);

            return context;
        }


        public static SentRequest<TRequest> Request<TRequest>(this ActorRef actor, TRequest request, UntypedActor sender)
        {
            Message<TRequest> message = actor.Request(request, sender.Self);

            return new SentRequestImpl<TRequest>(message, sender);
        }

        public static SentRequest<TRequest> Request<TRequest>(this ActorRef actor, UntypedActor sender)
        {
            Message<TRequest> message = actor.Request<TRequest>(sender.Self);

            return new SentRequestImpl<TRequest>(message, sender);
        }

        public static SentRequest<TRequest> Request<TRequest>(this ActorRef actor, object values, UntypedActor sender)
            where TRequest : class
        {
            Message<TRequest> message = actor.Request<TRequest>(values, sender.Self);

            return new SentRequestImpl<TRequest>(message, sender);
        }

        public static SentRequest<TRequest> Request<TRequest>(this ActorRef actor, object values, UntypedActor sender,
                                                              Action<SetMessageHeader> messageCallback)
            where TRequest : class
        {
            Message<TRequest> message = actor.Request<TRequest>(values, sender.Self, messageCallback);

            return new SentRequestImpl<TRequest>(message, sender);
        }
    }
}