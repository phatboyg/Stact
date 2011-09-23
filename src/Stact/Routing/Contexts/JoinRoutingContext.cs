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
    using MessageHeaders;


    public class JoinRoutingContext<T1, T2> :
        RoutingContext<Tuple<RoutingContext<T1>, RoutingContext<T2>>>,
        RoutingContext<Message<Tuple<RoutingContext<T1>, RoutingContext<T2>>>>
    {
        readonly RoutingContext<T1> _left;
        readonly RoutingContext<T2> _right;
        Tuple<RoutingContext<T1>, RoutingContext<T2>> _body;
        Message<Tuple<RoutingContext<T1>, RoutingContext<T2>>> _message;
        int _priority;

        public JoinRoutingContext(RoutingContext<T1> left, RoutingContext<T2> right)
        {
            _left = left;
            _right = right;
            _body = new Tuple<RoutingContext<T1>, RoutingContext<T2>>(left, right);
            _message = new MessageImpl<Tuple<RoutingContext<T1>, RoutingContext<T2>>>(_body);

            // this is arbitrary, but the point is to make it more important that a single receive of the same type
            _priority = left.Priority + right.Priority;
        }

        Message<Tuple<RoutingContext<T1>, RoutingContext<T2>>>
            RoutingContext<Message<Tuple<RoutingContext<T1>, RoutingContext<T2>>>>.Body
        {
            get { return _message; }
        }

        public void Match(
            Action<RoutingContext<Message<Message<Tuple<RoutingContext<T1>, RoutingContext<T2>>>>>> messageCallback,
            Action<RoutingContext<Request<Message<Tuple<RoutingContext<T1>, RoutingContext<T2>>>>>> requestCallback,
            Action<RoutingContext<Response<Message<Tuple<RoutingContext<T1>, RoutingContext<T2>>>>>> responseCallback)
        {
            throw new StactException("Nesting of header interfaces is not supported.");
        }

        public bool IsAlive
        {
            get { return _left.IsAlive && _right.IsAlive; }
        }

        public void Evict()
        {
            _left.Evict();
            _right.Evict();
        }

        public Tuple<RoutingContext<T1>, RoutingContext<T2>> Body
        {
            get { return _body; }
        }

        public int Priority
        {
            get { return _priority; }
        }

        public void Match(
            Action<RoutingContext<Message<Tuple<RoutingContext<T1>, RoutingContext<T2>>>>> messageCallback,
            Action<RoutingContext<Request<Tuple<RoutingContext<T1>, RoutingContext<T2>>>>> requestCallback,
            Action<RoutingContext<Response<Tuple<RoutingContext<T1>, RoutingContext<T2>>>>> responseCallback)
        {
            messageCallback(this);
        }

        public void Convert<TResult>(Action<RoutingContext<TResult>> callback)
        {
            throw new StactException("Projects are not supported in join networks.");
        }
    }
}