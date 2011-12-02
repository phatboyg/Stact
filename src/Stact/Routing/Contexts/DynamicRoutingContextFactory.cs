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
namespace Stact.Routing.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Caching;


    public class DynamicRoutingContextFactory :
        RoutingContextFactory
    {
        readonly Cache<Type, RoutingContextFactory> _messageFactoryCache;
        readonly Cache<Type, RoutingContextFactory> _objectFactoryCache;
        readonly Cache<Type, RoutingContextFactory> _typeFactoryCache;

        public DynamicRoutingContextFactory()
        {
            _typeFactoryCache = new ConcurrentCache<Type, RoutingContextFactory>(CreateMissingContextFactory);
            _messageFactoryCache = new GenericTypeCache<RoutingContextFactory>(typeof(MessageRoutingContextFactory<>));
            _objectFactoryCache = new GenericTypeCache<RoutingContextFactory>(typeof(ObjectRoutingContextFactory<>));
        }

        public void Create(object message, Activation activation)
        {
            if (message == null)
                throw new ArgumentNullException("message", "A null message was specified.");

            _typeFactoryCache[message.GetType()].Create(message, activation);
        }

        RoutingContextFactory CreateMissingContextFactory(Type type)
        {
            return IsMessage(type)
                .Concat(IsMessageClass(type))
                .Concat(IsObject(type))
                .First();
        }

        IEnumerable<RoutingContextFactory> IsMessageClass(Type type)
        {
            Type bodyType = type.GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Message<>))
                .Select(x => x.GetGenericArguments()[0])
                .FirstOrDefault();
            if (bodyType != null)
                yield return _messageFactoryCache[bodyType];
        }

        IEnumerable<RoutingContextFactory> IsMessage(Type type)
        {
            bool matches = type.IsInterface && type.IsGenericType
                           && type.GetGenericTypeDefinition() == typeof(Message<>);

            if (matches)
                yield return _messageFactoryCache[type.GetGenericArguments()[0]];
        }


        IEnumerable<RoutingContextFactory> IsObject(Type type)
        {
            yield return _objectFactoryCache[type];
        }
    }
}