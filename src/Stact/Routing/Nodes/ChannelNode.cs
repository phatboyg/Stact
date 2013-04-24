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
namespace Stact.Routing.Nodes
{
    using System;
    using System.Collections.Generic;


    public class ChannelNode<TChannel> :
        Activation<TChannel>
    {
        readonly Channel<TChannel> _channel;

        public ChannelNode(Channel<TChannel> channel, IList<IDisposable> disposables)
        {
            _channel = channel;
        }

        public Channel<TChannel> Output
        {
            get { return _channel; }
        }

        public ActivationType ActivationType
        {
            get { return ActivationType.ChannelNode; }
        }

        public bool Enabled
        {
            get { return true; }
        }

        public void Activate(RoutingContext<TChannel> context)
        {
            _channel.Send(context.Body);
        }

        public void Match<T>(Action<T> callback)
            where T : class
        {
            var match = this as T;
            if (match != null)
                callback(match);
        }
    }
}