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
    using System.Collections.Generic;


    public abstract class Memory<T>
    {
        readonly ActiveContextList<T> _contexts;
        readonly ActivationList<T> _successors;

        public Memory()
        {
            _contexts = new ActiveContextList<T>();
            _successors = new ActivationList<T>();
        }


        public bool Enabled
        {
            get { return true; }
        }

        public int Count
        {
            get { return _contexts.Count; }
        }

        public void Activate(RoutingContext<T> context)
        {
            Add(context);
        }

        public void RightActivate(Func<RoutingContext<T>, bool> callback)
        {
            All(callback);
        }

        public void RightActivate(RoutingContext<T> context, Action<RoutingContext<T>> callback)
        {
            Any(context, callback);
        }

        public IEnumerable<Activation<T>> Successors
        {
            get { return _successors; }
        }

        protected void Add(RoutingContext<T> message)
        {
            _contexts.Add(message);
        }

        protected void All(Func<RoutingContext<T>, bool> callback)
        {
            _contexts.All(callback);
        }

        protected void Any(RoutingContext<T> match, Action<RoutingContext<T>> callback)
        {
            _contexts.Any(match, callback);
        }

        public void AddActivation(Activation<T> activation)
        {
            _successors.Add(activation);

            _contexts.All(context =>
                {
                    if (!activation.Enabled)
                        return false;

                    if (context.IsAlive)
                        activation.Activate(context);

                    return true;
                });
        }

        public void RemoveActivation(Activation<T> activation)
        {
            _successors.Remove(activation);
        }
    }
}