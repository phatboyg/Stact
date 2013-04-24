// Copyright 2010-2013 Chris Patterson
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
    using Internals.Extensions;
    using MessageHeaders;


    public static class ActorSendExtensions
    {
        public static Message<T> Send<T>(this ActorRef actor)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var message = DynamicProxyFactory.Get<T>();

            var context = new MessageContext<T>(message);

            actor.Send(context);

            return context;
        }

        public static Message<T> Send<T>(this ActorRef actor, Action<SetMessageHeader> setMessageHeaderCallback)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var message = DynamicProxyFactory.Get<T>();

            var context = new MessageContext<T>(message);
            setMessageHeaderCallback(context);

            actor.Send(context);

            return context;
        }

        public static Message<T> Send<T>(this ActorRef actor, ActorRef sender)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var message = DynamicProxyFactory.Get<T>();

            var context = new MessageContext<T>(message, sender);

            actor.Send(context);

            return context;
        }

        public static Message<T> Send<T>(this ActorRef actor, ActorRef sender,
            Action<SetMessageHeader> setMessageHeaderCallback)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var message = DynamicProxyFactory.Get<T>();

            var context = new MessageContext<T>(message, sender);
            setMessageHeaderCallback(context);

            actor.Send(context);

            return context;
        }

        public static Message<T> Send<T>(this ActorRef actor, T message)
        {
            var context = new MessageContext<T>(message);

            actor.Send(context);

            return context;
        }

        public static Message<T> Send<T>(this ActorRef actor, T message,
            Action<SetMessageHeader> setMessageHeaderCallback)
        {
            var context = new MessageContext<T>(message);
            setMessageHeaderCallback(context);

            actor.Send(context);

            return context;
        }

        public static Message<T> Send<T>(this ActorRef actor, T message, ActorRef sender)
        {
            var context = new MessageContext<T>(message, sender);

            actor.Send(context);

            return context;
        }

        public static Message<T> Send<T>(this ActorRef actor, T message, ActorRef sender,
            Action<SetMessageHeader> setMessageHeaderCallback)
        {
            var context = new MessageContext<T>(message, sender);
            setMessageHeaderCallback(context);

            actor.Send(context);

            return context;
        }

        public static Message<T> Send<T>(this ActorRef actor, Message<T> message,
            Action<SetMessageHeader> setMessageHeaderCallback)
        {
            var context = message as MessageContext<T>;
            if (context == null)
                throw new ArgumentException("Unexpected message context type: " + message.GetType().GetTypeName());

            setMessageHeaderCallback(context);

            actor.Send(context);

            return context;
        }
    }
}