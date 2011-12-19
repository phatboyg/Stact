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
namespace Stact.Internal
{
    using Magnum;


    public class ActorExceptionHandlerHandle<TState> :
        ExceptionHandlerHandle
    {
        readonly WeakReference<StactActor<TState>> _actor;
        readonly ActorExceptionHandler _exceptionHandler;

        public ActorExceptionHandlerHandle(StactActor<TState> actor, ActorExceptionHandler exceptionHandler)
        {
            _actor = new WeakReference<StactActor<TState>>(actor);
            _exceptionHandler = exceptionHandler;
        }

        public void Cancel()
        {
            if (_actor.IsAlive)
                _actor.Target.RemoveExceptionHandler(_exceptionHandler);
        }
    }
}