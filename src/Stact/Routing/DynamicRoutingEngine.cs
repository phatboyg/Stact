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
namespace Stact.Routing
{
    using System;
    using Configuration;
    using Configuration.Internal;
    using Internal;
    using Stact.Internal;


    public class DynamicRoutingEngine :
        RoutingEngine
    {
        static readonly DynamicRoutingContextFactory _contextFactory = new DynamicRoutingContextFactory();

        readonly DynamicRoutingEngineConfigurator _configurator;
        readonly Fiber _fiber;
        readonly OperationList _operationList;
        readonly Activation _root;
        bool _shutdown;

        public DynamicRoutingEngine(Fiber fiber)
        {
            _fiber = fiber;

            _operationList = new OperationList();
            _root = new RootNode();

            _configurator = new DynamicRoutingEngineConfigurator(this);
        }

        public Activation Root
        {
            get { return _root; }
        }

        public void Send<T>(T message)
        {
            _fiber.Add(() =>
                {
                    _contextFactory.Create(message, _root);
                    _operationList.Run();
                });
        }

        public void Add(Action action)
        {
            if (_shutdown)
                return;

            _operationList.Add(action);
            _fiber.Add(() => _operationList.Run());
        }

        public void Shutdown()
        {
            _shutdown = true;
        }

        public void Configure(Action<RoutingEngineConfigurator> callback)
        {
            _fiber.Add(() =>
                {
                    DynamicRoutingEngineConfigurator configurator = _configurator;

                    callback(configurator);
                });
        }
    }
}