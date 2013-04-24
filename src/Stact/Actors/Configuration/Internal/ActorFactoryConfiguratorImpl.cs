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
namespace Stact.Configuration.Internal
{
    using System;
    using System.Collections.Generic;
    using Actors.Behaviors;
    using Configurators;
    using Conventions;
    using Stact.Internal;


    public class ActorFactoryConfiguratorImpl<TState> :
        FiberFactoryConfiguratorImpl<ActorFactoryConfigurator<TState>>,
        ActorFactoryConfigurator<TState>
    {
        readonly BehaviorConvention[] _conventions;
        SchedulerFactory _schedulerFactory;

        public ActorFactoryConfiguratorImpl()
        {
            UseSharedScheduler();

            _conventions = new BehaviorConvention[]
                {
                    new MessageWithSenderMethodBehaviorConvention(),
                    new MessageOnlyMethodBehaviorConvention(),
                    new ExceptionHandlerMethodBehaviorConvention(),
                    new ExitHandlerMethodBehaviorConvention(),
                };
        }

        public ActorFactoryConfigurator<TState> SetExitTimeout(TimeSpan timeout)
        {
            SetStopTimeout(timeout);

            return this;
        }

        public ActorFactoryConfigurator<TState> UseSharedScheduler()
        {
            _schedulerFactory = () => new TimerScheduler(new PoolFiber());

            return this;
        }

        public ActorFactoryConfigurator<TState> UseScheduler(Scheduler scheduler)
        {
            _schedulerFactory = () => scheduler;

            return this;
        }

        public ActorFactoryConfigurator<TState> UseSchedulerFactory(SchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;

            return this;
        }

        public override IEnumerable<ValidateConfigurationResult> ValidateConfiguration()
        {
            if (_schedulerFactory == null)
                yield return this.Failure("SchedulerFactory", "must be specified");

            foreach (ValidateConfigurationResult result in base.ValidateConfiguration())
                yield return result;
        }

        public ActorFactory<TState> Configure()
        {
            ActorBehaviorFactory<TState> factory =
                new ActorBehaviorFactoryImpl<TState>(_conventions);

            return new ActorFactoryImpl<TState>(GetConfiguredFiberFactoryEx(), _schedulerFactory, factory);
        }
    }
}