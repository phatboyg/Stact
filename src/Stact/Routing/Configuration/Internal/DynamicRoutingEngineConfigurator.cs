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
namespace Stact.Routing.Configuration.Internal
{
    using System;
    using Magnum.Collections;
    using Magnum.Extensions;
    using Routing.Internal;


    public class DynamicRoutingEngineConfigurator :
        RoutingEngineConfigurator
    {
        static readonly DynamicRoutingContextFactory _contextFactory = new DynamicRoutingContextFactory();


        readonly DynamicRoutingEngine _engine;
        readonly Cache<Type, object> _joinNodes;

        public DynamicRoutingEngineConfigurator(DynamicRoutingEngine engine)
        {
            _engine = engine;
            _joinNodes = new Cache<Type, object>();
        }

        public RemoveActivation Add<T>(Activation<T> activation)
        {
            BetaMemory<T> joinNode = GetJoinNode<T>();

            joinNode.AddActivation(activation);

            return () =>
            {
                joinNode.RemoveActivation(activation);
            };
        }

        public RoutingEngine Engine
        {
            get { return _engine; }
        }

        public RemoveActivation Add<T1, T2>(Activation<System.Tuple<T1, T2>> activation)
        {
            BetaMemory<System.Tuple<T1, T2>> joinNode = GetJoinNode<T1, T2>();

            joinNode.AddActivation(activation);

            return () =>
            {
                joinNode.RemoveActivation(activation);
            };
        }

        BetaMemory<T> GetJoinNode<T>()
        {
            return (BetaMemory<T>)_joinNodes.Retrieve(typeof(T), x => FindJoinNode<T>());
        }

        BetaMemory<System.Tuple<T1, T2>> GetJoinNode<T1, T2>()
        {
            return (BetaMemory<System.Tuple<T1, T2>>)_joinNodes.Retrieve(typeof(System.Tuple<T1, T2>),
                                                                         x => FindJoinNode<T1, T2>());
        }

        object FindJoinNode<T>()
        {
            JoinNode<T> result = null;

            new MatchAlphaNode<T>(_engine, alphaNode =>
            {
                new MatchJoinNode<T>(alphaNode, joinNode =>
                {
                    result = joinNode;
                });
            });

            if (result == null)
                throw new InvalidOperationException("Failed to create join node: " + typeof(T).ToShortTypeName());

            return result.BetaMemory;
        }

        BetaMemory<System.Tuple<T1, T2>> FindJoinNode<T1, T2>()
        {
            JoinNode<T1, T2> result = null;
            new MatchJoinNode<T1, T2>(_engine, joinNode =>
            {
                result = joinNode;
            });

            if (result == null)
                throw new InvalidOperationException("Failed to create join node: " + typeof(System.Tuple<T1, T2>).ToShortTypeName());

            return result.BetaMemory;
        }
    }
}