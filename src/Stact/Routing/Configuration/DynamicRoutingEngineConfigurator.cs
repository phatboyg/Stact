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
namespace Stact.Routing.Configuration
{
    using System;
    using Internal;
    using Magnum.Caching;
    using Magnum.Extensions;
    using Nodes;


    public class DynamicRoutingEngineConfigurator :
        RoutingEngineConfigurator
    {
        readonly ConsumerNodeFactory _consumerFactory = new DynamicConsumerNodeFactory();

        readonly DynamicRoutingEngine _engine;
        readonly Cache<Type, object> _joinNodes;

        public DynamicRoutingEngineConfigurator(DynamicRoutingEngine engine)
        {
            _engine = engine;
            _joinNodes = new DictionaryCache<Type, object>();
        }

        public RemoveActivation Add<T>(Activation<T> activation)
        {
            BetaMemory<T> joinNode = GetJoinNode<T>();

            joinNode.AddActivation(activation);

            return () => { joinNode.RemoveActivation(activation); };
        }

        public RemoveActivation Receive<T>(Consumer<Message<T>> consumer)
        {
            return _consumerFactory.Create(consumer, this);
        }

        public RemoveActivation SelectiveReceive<T>(SelectiveConsumer<Message<T>> consumer)
        {
            return _consumerFactory.Create(consumer, this);
        }

        public RoutingEngine Engine
        {
            get { return _engine; }
        }

        BetaMemory<T> GetJoinNode<T>()
        {
            return (BetaMemory<T>)_joinNodes.Get(typeof(T), x => FindJoinNode<T>());
        }

        object FindJoinNode<T>()
        {
            JoinNode<T> result = null;

            new MatchAlphaNode<T>(_engine,
                                  alphaNode => { new MatchJoinNode<T>(alphaNode, joinNode => { result = joinNode; }); });

            if (result == null)
                throw new InvalidOperationException("Failed to create join node: " + typeof(T).ToShortTypeName());

            return result.BetaMemory;
        }
    }
}