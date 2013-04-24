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
    using Internals.Caching;


    public class MessageRoutingContext<T> :
        RoutingContext<T>
    {
        static readonly Cache<Type, RoutingContextProxyFactory<T>> _factoryCache =
            new DictionaryCache<Type, RoutingContextProxyFactory<T>>(CreateMissingProxyFactory);

        readonly Message<T> _message;
        readonly int _priority;
        bool _alive = true;

        public MessageRoutingContext(Message<T> message, int priority = 0)
        {
            _message = message;
            _priority = priority;
        }

        public bool IsAlive
        {
            get { return _alive; }
        }

        public void Evict()
        {
            _alive = false;
        }

        public int Priority
        {
            get { return _priority; }
        }

        public void Convert<TResult>(Action<RoutingContext<TResult>> callback)
        {
            RoutingContext<TResult> proxy = _factoryCache[typeof(TResult)].CreateProxy<TResult>(this, _message);
            callback(proxy);
        }

        public T Body
        {
            get { return _message.Body; }
        }

        public Message<T> Message
        {
            get { return _message; }
        }

        static RoutingContextProxyFactory<T> CreateMissingProxyFactory(Type key)
        {
            return (RoutingContextProxyFactory<T>)
                   Activator.CreateInstance(typeof(MessageRoutingContextProxyFactory<,>).MakeGenericType(typeof(T), key));
        }
    }
}