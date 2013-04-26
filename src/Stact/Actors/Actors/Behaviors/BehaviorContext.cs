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
    public interface BehaviorContext<TState, out TBehavior>
        where TBehavior : Behavior<TState>
    {
        /// <summary>
        /// Adds a message consumer to the actor inbox. The consumer is bound to the behavior,
        /// so if the behavior is removed the receive is cancelled.
        /// </summary>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <param name="consumer">The message consumer</param>
        void Receive<TMessage>(Consumer<Message<TMessage>> consumer);

        /// <summary>
        /// Pushes an exception handler on the stack for the actor. 
        /// </summary>
        /// <param name="handler"></param>
        void SetExceptionHandler(ActorExceptionHandler handler);
        
        /// <summary>
        /// Pushes an exit handler on the stack for the actor.
        /// </summary>
        /// <param name="handler"></param>
        void SetExitHandler(ActorExitHandler handler);

        /// <summary>
        /// The behavior instance
        /// </summary>
        TBehavior Behavior { get; }
    }
}