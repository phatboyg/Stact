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
namespace Stact.Routing
{
    using System;
    using Configuration;
    using Contexts;
    using Nodes;
    using Stact.Internal;


    public class MessageRoutingEngine :
        RoutingEngine
    {
        readonly MessageRoutingEngineAgenda _agenda;

        readonly MessageRoutingEngineConfigurator _configurator;
        readonly Activation _root;
        bool _shutdown;

        public MessageRoutingEngine()
        {
            _agenda = new MessageRoutingEngineAgenda();
            _root = new RootNode();

            _configurator = new MessageRoutingEngineConfigurator(this, _agenda);
        }

        public Activation Root
        {
            get { return _root; }
        }

        public void Send<T>(Message<T> message)
        {
            RoutingContext<T> context = new MessageRoutingContext<T>(message);
            _root.Activate(context);

            _agenda.Run();
        }

        public void Add(int priority, Action action)
        {
            if (_shutdown)
                return;

            _agenda.Add(priority, action);
        }

        public void Shutdown()
        {
            _shutdown = true;
            _agenda.Stop();
        }

        public void Configure(Action<RoutingEngineConfigurator> callback)
        {
            MessageRoutingEngineConfigurator configurator = _configurator;

            callback(configurator);

            _agenda.Run();
        }

        public void DispatchMessage<T>(Message<T> message)
        {
            RoutingContext<T> context = new MessageRoutingContext<T>(message);
            _root.Activate(context);

            _agenda.Run();
        }
    }
}