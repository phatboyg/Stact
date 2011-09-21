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
    public interface RequestRoutingContextProxyFactory<T>
    {
        RoutingContext<TOutput> CreateProxy<TOutput>(RoutingContext<T> input, Request<T> request);
    }


    public class RequestRoutingContextProxyFactory<TInput, TOutput> :
        RequestRoutingContextProxyFactory<TInput>
        where TInput : TOutput
    {
        public RoutingContext<TResult> CreateProxy<TResult>(RoutingContext<TInput> input, Request<TInput> request)
        {
            var proxy = new RequestRoutingContextProxy<TInput, TOutput>(input, request);

            return proxy as RoutingContext<TResult>;
        }
    }
}