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
    using Magnum.Extensions;
    using Magnum.Reflection;
    using MessageHeaders;


    public static class ActorSendExtensions
    {
        public static Message<T> ToMessage<T>(this T body)
        {
            var context = new MessageContext<T>(body);

            return context;
        }

        public static Message<T> ToMessage<T>(this T body, ActorRef sender)
        {
            var context = new MessageContext<T>(body, sender);

            return context;
        }

        public static Message<T> Send<T>(this ActorRef actor, T message,
                                         Action<SetMessageHeader> messageCallback)
        {
            var context = new MessageContext<T>(message);
            messageCallback(context);

            return Send(actor, context);
        }

        public static Message<T> Send<T>(this ActorRef actor, Message<T> message,
                                         Action<SetMessageHeader> messageCallback)
        {
            var context = message as MessageContext<T>;
            if (context == null)
                throw new ArgumentException("Unexpected message context type: " + message.GetType().ToShortTypeName());

            messageCallback(context);

            return Send(actor, context);
        }

//        public static Message<T> Send<T>(this ActorRef actor, object values)
//            where T : class
//        {
//            if (!typeof(T).IsInterface)
//                throw new ArgumentException("Default Implementations can only be created for interfaces");
//
//            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);
//            var context = new MessageContext<T>(message);
//
//            return Send(actor, context);
//        }
//
//        public static Message<T> Send<T>(this ActorRef actor, object values,
//                                         Action<SetMessageHeader> messageCallback)
//            where T : class
//        {
//            if (!typeof(T).IsInterface)
//                throw new ArgumentException("Default Implementations can only be created for interfaces");
//
//            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);
//            var context = new MessageContext<T>(message);
//            messageCallback(context);
//
//            return Send(actor, context);
//        }

        public static Message<T> Send<T>(this ActorRef actor)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            Type messageType = InterfaceImplementationBuilder.GetProxyFor(typeof(T));
            var message = (T)FastActivator.Create(messageType);

            var context = new MessageContext<T>(message);

            return Send(actor, context);
        }

        public static Message<T> Send<T>(this ActorRef actor, Action<SetMessageHeader> messageCallback)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            Type messageType = InterfaceImplementationBuilder.GetProxyFor(typeof(T));
            var message = (T)FastActivator.Create(messageType);

            var context = new MessageContext<T>(message);
            messageCallback(context);

            return Send(actor, context);
        }

        static Message<T> Send<T>(ActorRef actor, Message<T> message)
        {
            actor.Send(message);

            return message;
        }
    }
}