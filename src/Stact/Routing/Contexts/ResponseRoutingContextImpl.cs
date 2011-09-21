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
    using Magnum.Caching;
    using Magnum.Reflection;


    public class ResponseRoutingContextImpl<T> :
        AbstractRoutingContext,
        RoutingContext<Response<T>>,
        RoutingContext<T>
    {
        static readonly Cache<Type, RoutingContextProxyFactory<T>> _proxyFactoryCache =
            new ConcurrentCache<Type, RoutingContextProxyFactory<T>>(CreateMissingProxyFactory);

        readonly Response<T> _response;
        readonly int _priority;

        public ResponseRoutingContextImpl(Response<T> response, int priority = 0)
        {
            _response = response;
            _priority = priority;
        }

        public Response<T> Body
        {
            get { return _response; }
        }

        int RoutingContext<T>.Priority
        {
            get { return _priority; }
        }

        int RoutingContext<Response<T>>.Priority
        {
            get { return _priority; }
        }

        public void Match(Action<RoutingContext<Message<Response<T>>>> messageCallback,
                          Action<RoutingContext<Request<Response<T>>>> requestCallback,
                          Action<RoutingContext<Response<Response<T>>>> responseCallback)
        {
            throw new StactException("Nesting of header interfaces is not supported.");
        }

        void RoutingContext<Response<T>>.Convert<TResult>(Action<RoutingContext<TResult>> callback)
        {
            RoutingContext<TResult> proxy = _proxyFactoryCache[typeof(TResult)].CreateProxy<TResult>(this);
            callback(proxy);
        }

        public void Match(Action<RoutingContext<Message<T>>> messageCallback,
                          Action<RoutingContext<Request<T>>> requestCallback,
                          Action<RoutingContext<Response<T>>> responseCallback)
        {
            responseCallback(this);
        }

        void RoutingContext<T>.Convert<TResult>(Action<RoutingContext<TResult>> callback)
        {
            RoutingContext<TResult> proxy = _proxyFactoryCache[typeof(TResult)].CreateProxy<TResult>(this);
            callback(proxy);
        }

        T RoutingContext<T>.Body
        {
            get { return _response.Body; }
        }

        static RoutingContextProxyFactory<T> CreateMissingProxyFactory(Type key)
        {
            return
                (RoutingContextProxyFactory<T>)
                FastActivator.Create(typeof(ResponseRoutingContextProxyFactory<,>), new[] {typeof(T), key});
        }
    }
}