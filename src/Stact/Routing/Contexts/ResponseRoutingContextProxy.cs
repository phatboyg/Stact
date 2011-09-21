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


    public class ResponseRoutingContextProxy<TInput, TOutput> :
        RoutingContext<Response<TOutput>>,
        RoutingContext<TOutput>
        where TInput : TOutput
    {
        readonly RoutingContext<TInput> _input;
        readonly int _priority;

        public ResponseRoutingContextProxy(RoutingContext<TInput> input)
        {
            _input = input;
            _priority = input.Priority - 1000;
        }

        public bool IsAlive
        {
            get { return _input.IsAlive; }
        }

        public void Evict()
        {
            _input.Evict();
        }

        public Response<TOutput> Body
        {
            get { return (Response<TOutput>)_input.Body; }
        }

        int RoutingContext<TOutput>.Priority
        {
            get { return _priority; }
        }

        int RoutingContext<Response<TOutput>>.Priority
        {
            get { return _priority; }
        }

        public void Match(Action<RoutingContext<Message<Response<TOutput>>>> messageCallback,
                          Action<RoutingContext<Request<Response<TOutput>>>> requestCallback,
                          Action<RoutingContext<Response<Response<TOutput>>>> responseCallback)
        {
            throw new StactException("Nesting of header interfaces is not supported.");
        }

        public void Convert<TResult>(Action<RoutingContext<TResult>> callback)
        {
            _input.Convert(callback);
        }

        TOutput RoutingContext<TOutput>.Body
        {
            get { return _input.Body; }
        }

        public void Match(Action<RoutingContext<Message<TOutput>>> messageCallback,
                          Action<RoutingContext<Request<TOutput>>> requestCallback,
                          Action<RoutingContext<Response<TOutput>>> responseCallback)
        {
            responseCallback(this);
        }
    }
}