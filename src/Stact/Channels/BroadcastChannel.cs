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
namespace Stact
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// A BroadcastChannel sends a message to zero or more listeners
    /// </summary>
    public class BroadcastChannel :
        UntypedChannel
    {
        readonly UntypedChannel[] _listeners;

        public BroadcastChannel(IEnumerable<UntypedChannel> listeners)
        {
            if (listeners == null)
                throw new ArgumentNullException("listeners");

            _listeners = listeners.ToArray();
        }

        public BroadcastChannel(UntypedChannel[] listeners)
        {
            if (listeners == null)
                throw new ArgumentNullException("listeners");

            _listeners = listeners;
        }

        public IEnumerable<UntypedChannel> Listeners
        {
            get { return _listeners; }
        }

        public void Send<T>(T message)
        {
            for (int i = 0; i < _listeners.Length; i++)
                _listeners[i].Send(message);
        }
    }


    /// <summary>
    /// A BroadcastChannel sends a message to zero or more listeners
    /// </summary>
    /// <typeparam name = "T">Channel type</typeparam>
    public class BroadcastChannel<T> :
        Channel<T>
    {
        readonly Channel<T>[] _listeners;

        public BroadcastChannel(IEnumerable<Channel<T>> listeners)
        {
            if (listeners == null)
                throw new ArgumentNullException("listeners");

            _listeners = listeners.ToArray();
        }

        public BroadcastChannel(params Channel<T>[] listeners)
        {
            if (listeners == null)
                throw new ArgumentNullException("listeners");

            _listeners = listeners;
        }

        public IEnumerable<Channel<T>> Listeners
        {
            get { return _listeners; }
        }

        public void Send(T message)
        {
            for (int i = 0; i < _listeners.Length; i++)
                _listeners[i].Send(message);
        }
    }
}