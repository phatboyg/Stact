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
namespace Stact.Routing.Nodes
{
    using System;
    using Stact.Internal;


    /// <summary>
    /// The basic functionality of a production node that deals with
    /// evicting the message from the routing engine and dispatching it
    /// to the specified delegate
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class ProductionNode<T>
    {
        readonly RoutingEngineAgenda _agenda;
        readonly bool _disableOnActivation;
        bool _enabled;

        protected ProductionNode(RoutingEngineAgenda agenda, bool disableOnActivation)
        {
            _agenda = agenda;
            _disableOnActivation = disableOnActivation;
            _enabled = true;
        }

        public bool Enabled
        {
            get { return _enabled; }
        }

        protected void Accept(RoutingContext<T> context, Action<Message<T>> callback)
        {
            _agenda.Add(context.Priority, () =>
                {
                    if (!context.IsAlive)
                        return;

                    Message<T> message = context.Message;

                    context.Evict();

                    if (_disableOnActivation)
                        _enabled = false;

                    callback(message);
                });
        }
    }
}