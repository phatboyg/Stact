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


    public static class SendExtensions
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

        public static Message<T> Send<T>(this UntypedChannel channel, object values)
            where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("Default Implementations can only be created for interfaces");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);
            var messageImpl = new MessageImpl<T>(message);

            channel.Send<Message<T>>(messageImpl);

            return messageImpl;
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
    }
}