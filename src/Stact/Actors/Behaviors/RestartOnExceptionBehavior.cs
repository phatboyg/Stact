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
namespace Stact.Behaviors
{
    using System;


    /// <summary>
    /// This is an idea at this point. A way to define a behavior that can be applied
    /// alongside the standard actor behaviors to handle things like unhandled exceptions
    /// and other events, such as timeouts. Also thinking towards how linking and supervision
    /// would be applied as behaviors
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public class RestartOnExceptionBehavior<TState> :
        Behavior<TState>
    {
        readonly Actor<TState> _actor;
        readonly TimeSpan _restartInterval;
        readonly int _restartLimit;

        public RestartOnExceptionBehavior(Actor<TState> actor)
        {
            _actor = actor;

            // seems to me, that we should deep copy the state or something so that if the 
            // actor is restarted, it starts with an original state instead of any changes that 
            // have occurred. Or maybe even new up an entirely new state for the actor.
        }


        public RestartOnExceptionBehavior(Actor<TState> actor, int restartLimit, TimeSpan restartInterval)
            : this(actor)
        {
            _restartLimit = restartLimit;
            _restartInterval = restartInterval;
        }

        public void HandleException(Exception exception, NextExceptionHandler next)
        {
            // this should kill the existing actor and start up a new one

            throw new NotImplementedException("The actor restart has not been implemented yet.");
        }
    }
}