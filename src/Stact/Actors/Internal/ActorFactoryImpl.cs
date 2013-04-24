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
namespace Stact.Internal
{
    using System;
    using Actors.Behaviors;
    using Executors;
    using Internals.Extensions;


    public class ActorFactoryImpl<TState> :
        ActorFactory<TState>
    {
        readonly ActorBehaviorFactory<TState> _actorBehaviorFactory;
        readonly FiberFactoryEx _fiberFactory;
        readonly SchedulerFactory _schedulerFactory;

        public ActorFactoryImpl(FiberFactoryEx fiberFactory, SchedulerFactory schedulerFactory,
            ActorBehaviorFactory<TState> actorBehaviorFactory)
        {
            if (fiberFactory == null)
                throw new ArgumentNullException("fiberFactory");
            if (schedulerFactory == null)
                throw new ArgumentNullException("schedulerFactory");
            if (actorBehaviorFactory == null)
                throw new ArgumentNullException("actorBehaviorFactory");

            _fiberFactory = fiberFactory;
            _schedulerFactory = schedulerFactory;
            _actorBehaviorFactory = actorBehaviorFactory;
        }

        public Actor<TState> New(TState state)
        {
            StactActor<TState> actor = null;

            Fiber fiber = _fiberFactory(new TryCatchOperationExecutor(ex => { actor.OnError(ex); }));

            Scheduler scheduler = _schedulerFactory();

            actor = new StactActor<TState>(fiber, scheduler, _actorBehaviorFactory, state);

            return actor;
        }

        public ActorRef New<T>(T state)
        {
            var factory = this as ActorFactory<T>;
            if (factory == null)
            {
                throw new ArgumentException("Factory of type " + typeof(TState).GetTypeName()
                                            + " cannot create actors for " + typeof(T).GetTypeName());
            }

            return factory.New(state).Self;
        }
    }
}