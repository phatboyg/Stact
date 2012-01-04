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


    public class ActorBehaviorHandle<TState, TBehavior> :
        BehaviorHandle,
        BehaviorContext<TState, TBehavior>
        where TBehavior : Behavior<TState>
    {
        readonly Actor<TState> _actor;
        readonly TBehavior _behavior;
        readonly HashSet<ReceiveHandle> _receives;
        ExceptionHandlerHandle _exceptionHandler;
        ExitHandlerHandle _exitHandler;

        public ActorBehaviorHandle(Actor<TState> actor, TBehavior behavior)
        {
            _actor = actor;
            _behavior = behavior;
            _receives = new HashSet<ReceiveHandle>();
        }

        public void Receive<TMessage>(Consumer<Message<TMessage>> consumer)
        {
            ReceiveHandle receive = null;
            Consumer<Message<TMessage>> nestedConsumer = null;
            nestedConsumer = msg =>
                {
                    if (receive != null)
                        _receives.Remove(receive);

                    receive = _actor.Receive(nestedConsumer);
                    _receives.Add(receive);

                    consumer(msg);
                };

            receive = _actor.Receive(nestedConsumer);
            _receives.Add(receive);
        }

        public void SetExceptionHandler(ActorExceptionHandler handler)
        {
            _exceptionHandler = _actor.SetExceptionHandler(handler);
        }

        public void SetExitHandler(ActorExitHandler handler)
        {
            _exitHandler = _actor.SetExitHandler(handler);
        }

        public TBehavior Behavior
        {
            get { return _behavior; }
        }

        public void Remove()
        {
            foreach (ReceiveHandle receive in _receives)
                receive.Cancel();
            _receives.Clear();

            if (_exceptionHandler != null)
            {
                _exceptionHandler.Cancel();
                _exceptionHandler = null;
            }

            if (_exitHandler != null)
            {
                _exitHandler.Cancel();
                _exitHandler = null;
            }
        }
    }
}