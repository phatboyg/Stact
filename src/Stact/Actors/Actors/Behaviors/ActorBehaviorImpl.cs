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
    using System.Collections.Generic;
    using System.Linq;


    public class ActorBehaviorImpl<TState, TBehavior> :
        ActorBehavior<TState>
        where TBehavior : class, Behavior<TState>
    {
        readonly ActorBehaviorApplicator<TState, TBehavior>[] _applicators;
        readonly BehaviorFactory<TState, TBehavior> _behaviorFactory;

        public ActorBehaviorImpl(BehaviorFactory<TState, TBehavior> behaviorFactory,
                                 IEnumerable<ActorBehaviorApplicator<TState, TBehavior>> applicators)
        {
            _behaviorFactory = behaviorFactory;
            _applicators = applicators.ToArray();
        }

        public BehaviorHandle ApplyTo(Actor<TState> actor)
        {
            return _behaviorFactory.CreateBehavior(actor, behavior =>
                {
                    var behaviorHandle = new ActorBehaviorHandle<TState, TBehavior>(actor, behavior);

                    for (int i = 0; i < _applicators.Length; i++)
                        _applicators[i].Apply(behaviorHandle);

                    return behaviorHandle;
                });
        }
    }
}