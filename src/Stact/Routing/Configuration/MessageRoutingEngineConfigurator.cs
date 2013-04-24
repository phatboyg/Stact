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
namespace Stact.Routing.Configuration
{
    using System;
    using Internal;
    using Internals.Caching;
    using Internals.Extensions;
    using Nodes;


    public class MessageRoutingEngineConfigurator :
        RoutingEngineConfigurator
    {
        readonly RoutingEngineAgenda _agenda;

        readonly MessageRoutingEngine _engine;
        readonly Cache<Type, object> _joinNodes;

        public MessageRoutingEngineConfigurator(MessageRoutingEngine engine, RoutingEngineAgenda agenda)
        {
            _engine = engine;
            _agenda = agenda;
            _joinNodes = new DictionaryCache<Type, object>();
        }

        public RoutingEngineAgenda Agenda
        {
            get { return _agenda; }
        }

        public RemoveActivation AddActivation<T>(Activation<T> activation)
        {
            BetaMemory<T> joinNode = GetJoinNode<T>();

            joinNode.AddActivation(activation);

            return () => { joinNode.RemoveActivation(activation); };
        }

        public RemoveActivation Receive<T>(Consumer<Message<T>> consumer)
        {
            var consumerNode = new ConsumerNode<T>(_agenda, consumer);

            return AddActivation(consumerNode);
        }

        public RemoveActivation SelectiveReceive<T>(SelectiveConsumer<Message<T>> consumer)
        {
            var consumerNode = new SelectiveConsumerNode<T>(_agenda, consumer);

            return AddActivation(consumerNode);
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
                throw new InvalidOperationException("Failed to create join node: " + typeof(T).GetTypeName());

            return result.BetaMemory;
        }
    }
}