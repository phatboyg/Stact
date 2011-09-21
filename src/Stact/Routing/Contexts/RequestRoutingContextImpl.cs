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


    public class RequestRoutingContextImpl<T> :
        AbstractRoutingContext,
        RoutingContext<Request<T>>,
        RoutingContext<T>

    {
        static readonly Cache<Type, RequestRoutingContextProxyFactory<T>> _proxyFactoryCache =
            new ConcurrentCache<Type, RequestRoutingContextProxyFactory<T>>(CreateMissingProxyFactory);

        readonly int _priority;
        readonly Request<T> _request;

        public RequestRoutingContextImpl(Request<T> request, int priority = 0)
        {
            _request = request;
            _priority = priority;
        }

        Request<T> RoutingContext<Request<T>>.Body
        {
            get { return _request; }
        }

        int RoutingContext<Request<T>>.Priority
        {
            get { return _priority; }
        }

        public void Match(Action<RoutingContext<Message<Request<T>>>> messageCallback,
                          Action<RoutingContext<Request<Request<T>>>> requestCallback,
                          Action<RoutingContext<Response<Request<T>>>> responseCallback)
        {
            throw new StactException("Nesting of header interfaces is not supported.");
        }

        void RoutingContext<Request<T>>.Convert<TResult>(Action<RoutingContext<TResult>> callback)
        {
            RoutingContext<TResult> proxy = _proxyFactoryCache[typeof(TResult)].CreateProxy<TResult>(this, _request);
            callback(proxy);
        }

        int RoutingContext<T>.Priority
        {
            get { return _priority; }
        }

        public void Match(Action<RoutingContext<Message<T>>> messageCallback,
                          Action<RoutingContext<Request<T>>> requestCallback,
                          Action<RoutingContext<Response<T>>> responseCallback)
        {
            requestCallback(this);
        }

        void RoutingContext<T>.Convert<TResult>(Action<RoutingContext<TResult>> callback)
        {
            RoutingContext<TResult> proxy = _proxyFactoryCache[typeof(TResult)].CreateProxy<TResult>(this, _request);
            callback(proxy);
        }


        T RoutingContext<T>.Body
        {
            get { return _request.Body; }
        }

        static RequestRoutingContextProxyFactory<T> CreateMissingProxyFactory(Type key)
        {
            return
                (RequestRoutingContextProxyFactory<T>)
                FastActivator.Create(typeof(RequestRoutingContextProxyFactory<,>), new[] {typeof(T), key});
        }
    }
}