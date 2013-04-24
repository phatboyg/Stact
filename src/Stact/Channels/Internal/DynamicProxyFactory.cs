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
namespace Stact.Internal
{
    using System;
    using Internals.Caching;


    public static class DynamicProxyFactory
    {
        static readonly ImplementationBuilder _builder = new DynamicImplementationBuilder();
        static readonly Cache<Type, ProxyFactory> _proxyTypes = new ConcurrentCache<Type, ProxyFactory>();

        public static T Get<T>()
        {
            return _proxyTypes.Get(typeof(T), _ =>
                {
                    Type proxyType = _builder.GetImplementationType(typeof(T));

                    return new TypeProxyFactory<T>(proxyType);
                })
                              .Get<T>();
        }
    }
}