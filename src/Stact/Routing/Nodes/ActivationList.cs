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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class ActivationList<TChannel> :
        IEnumerable<Activation<TChannel>>
    {
        Stack<Activation<TChannel>> _activations;

        public ActivationList(params Activation<TChannel>[] activations)
        {
            _activations = new Stack<Activation<TChannel>>(activations);
        }

        public IEnumerator<Activation<TChannel>> GetEnumerator()
        {
            bool collectNeeded = false;
            foreach (var activation in _activations)
            {
                if (activation.Enabled == false)
                {
                    collectNeeded = true;
                    continue;
                }

                yield return activation;
            }

            if (collectNeeded)
                Collect();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void Collect()
        {
            _activations = new Stack<Activation<TChannel>>(_activations.Where(x => x.Enabled));
        }

        public void Add(Activation<TChannel> activation)
        {
            _activations.Push(activation);
        }

        public void All(Action<Activation<TChannel>> callback)
        {
            foreach (var activation in this)
                callback(activation);
        }

        public void Remove(Activation<TChannel> activation)
        {
            IEnumerable<Activation<TChannel>> remaining = _activations
                .Where(x => x.Enabled)
                .Where(x => x != activation);

            _activations = new Stack<Activation<TChannel>>(remaining);
        }
    }
}