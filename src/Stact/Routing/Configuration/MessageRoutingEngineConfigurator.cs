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
    using Internals.Caching;
    using Internals.Extensions;
    using Nodes;


    public class MessageRoutingEngineConfigurator :
        RoutingEngineConfigurator
    {
        readonly RoutingEngineAgenda _agenda;

        readonly MessageRoutingEngine _engine;
        readonly Cache<Type, Memory> _memories;

        public MessageRoutingEngineConfigurator(MessageRoutingEngine engine, RoutingEngineAgenda agenda)
        {
            _engine = engine;
            _agenda = agenda;

            _memories = new DictionaryCache<Type, Memory>();
        }

        public RoutingEngineAgenda Agenda
        {
            get { return _agenda; }
        }

        public RemoveActivation AddActivation<T>(Activation<T> activation)
        {
            Memory memory = GetAlphaNode<T>();

            memory.AddActivation(activation);

            return () => memory.RemoveActivation(activation);
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

        Memory<T> GetAlphaNode<T>()
        {
            return (Memory<T>)_memories.Get(typeof(T), x => FindAlphaNode<T>());
        }

        Memory FindAlphaNode<T>()
        {
            AlphaNode<T> result = null;

            var matchAlphaNode = new MatchAlphaNode<T>(_engine, alphaNode => { result = alphaNode; });

            if (result == null)
                throw new InvalidOperationException("Failed to create alpha node: " + typeof(T).GetTypeName());

            return result;
        }
    }
}