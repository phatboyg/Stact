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
namespace Stact.Actors.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Internals.Caching;
    using Internals.Extensions;


    public class ActorBehaviorFactoryImpl<TState> :
        ActorBehaviorFactory<TState>
    {
        readonly Cache<Type, ActorBehavior<TState>> _applicators;
        readonly IEnumerable<BehaviorConvention> _conventions;

        public ActorBehaviorFactoryImpl(IEnumerable<BehaviorConvention> conventions)
        {
            _applicators = new ReaderWriterLockedCache<Type, ActorBehavior<TState>>(new DictionaryCache<Type, ActorBehavior<TState>>());
            _conventions = conventions;
        }

        public ActorBehavior<TState> CreateActorBehavior<TBehavior>()
            where TBehavior : class, Behavior<TState>
        {
            return _applicators.Get(typeof(TBehavior), _ => CreateApplicator<TBehavior>());
        }

        ActorBehavior<TState> CreateApplicator<TBehavior>()
            where TBehavior : class, Behavior<TState>
        {
            try
            {
                var behaviorFactory = new BehaviorFactoryImpl<TState, TBehavior>();

                IEnumerable<ActorBehaviorApplicator<TState, TBehavior>> applicators = _conventions
                    .SelectMany(x => x.GetApplicators<TState, TBehavior>());

                return new ActorBehaviorImpl<TState, TBehavior>(behaviorFactory, applicators);
            }
            catch (Exception ex)
            {
                throw StactException.New(ex, "An exception occurred creating the applicator for {0} with actor type {1}",
                    typeof(TBehavior).GetTypeName(), typeof(TState).GetTypeName());
            }
        }
    }
}