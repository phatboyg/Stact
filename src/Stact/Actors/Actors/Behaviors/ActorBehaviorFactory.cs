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
    /// <summary>
    /// Used to create instances of ActorBehaviors. An instance is created every
    /// time an actor changes behavior, with the behavior instance being passed an
    /// Actor<typeparamref name="TState"/> to use.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public interface ActorBehaviorFactory<TState>
    {
        /// <summary>
        /// Creates an ActorBehavior for an actor
        /// </summary>
        /// <typeparam name="TBehavior">The type of behavior to create, which must match the state type of the actor</typeparam>
        /// <returns>An ActorBehavior instance</returns>
        ActorBehavior<TState> CreateActorBehavior<TBehavior>()
            where TBehavior : class, Behavior<TState>;
    }
}