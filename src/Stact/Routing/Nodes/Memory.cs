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


    public interface Memory
    {
        void AddActivation<T>(Activation<T> activation);
        void RemoveActivation<T>(Activation<T> activation);
    }

    public abstract class Memory<T> :
        Memory
    {
        readonly ActiveContextList<T> _contexts;
        readonly ActivationList<T> _successors;

        protected Memory()
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

        public IEnumerable<Activation<T>> Successors
        {
            get { return _successors; }
        }

        protected void Add(RoutingContext<T> message)
        {
            _contexts.Add(message);
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

        public void AddActivation<TContext>(Activation<TContext> activation)
        {
            var self = this as Memory<TContext>;
            if (self == null)
                throw new ArgumentException("The memory is not of the expected type");

            self.AddActivation(activation);
        }

        public void RemoveActivation<TContext>(Activation<TContext> activation)
        {
            var self = this as Memory<TContext>;
            if (self == null)
                throw new ArgumentException("The memory is not of the expected type");

            self.RemoveActivation(activation);
        }
    }
}