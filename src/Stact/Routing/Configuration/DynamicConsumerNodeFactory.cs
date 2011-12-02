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
namespace Stact.Routing.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Caching;


    public class DynamicConsumerNodeFactory :
        ConsumerNodeFactory
    {
        readonly Cache<Type, ConsumerNodeFactory> _messageFactoryCache;
        readonly Cache<Type, ConsumerNodeFactory> _objectFactoryCache;
        readonly Cache<Type, ConsumerNodeFactory> _typeFactoryCache;

        public DynamicConsumerNodeFactory()
        {
            _typeFactoryCache = new DictionaryCache<Type, ConsumerNodeFactory>(CreateMissingNodeFactory);
            _messageFactoryCache = new GenericTypeCache<ConsumerNodeFactory>(typeof(MessageConsumerNodeFactory<>));
            _objectFactoryCache = new GenericTypeCache<ConsumerNodeFactory>(typeof(ObjectConsumerNodeFactory<>));
        }

        public RemoveActivation Create<T>(Consumer<Message<T>> consumer, RoutingEngineConfigurator configurator)
        {
            return _typeFactoryCache[typeof(T)].Create(consumer, configurator);
        }

        public RemoveActivation Create<T>(SelectiveConsumer<Message<T>> consumer, RoutingEngineConfigurator configurator)
        {
            return _typeFactoryCache[typeof(T)].Create(consumer, configurator);
        }

        ConsumerNodeFactory CreateMissingNodeFactory(Type type)
        {
            return IsMessage(type)
                .Concat(IsMessageClass(type))
                .Concat(IsObject(type))
                .First();
        }

        IEnumerable<ConsumerNodeFactory> IsMessageClass(Type type)
        {
            Type bodyType = type.GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Message<>))
                .Select(x => x.GetGenericArguments()[0])
                .FirstOrDefault();
            if (bodyType != null)
                yield return _messageFactoryCache[bodyType];
        }

        IEnumerable<ConsumerNodeFactory> IsMessage(Type type)
        {
            bool matches = type.IsInterface && type.IsGenericType
                           && type.GetGenericTypeDefinition() == typeof(Message<>);

            if (matches)
                yield return _messageFactoryCache[type.GetGenericArguments()[0]];
        }

        IEnumerable<ConsumerNodeFactory> IsObject(Type type)
        {
            yield return _objectFactoryCache[type];
        }
    }
}