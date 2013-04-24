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
namespace Stact.Configuration
{
    using System;


    public interface ActorFactoryConfigurator<TState>
    {
        ActorFactoryConfigurator<TState> HandleOnCallingThread();
        ActorFactoryConfigurator<TState> HandleOnThreadPool();
        ActorFactoryConfigurator<TState> HandleOnThread();
        ActorFactoryConfigurator<TState> HandleOnFiber(Fiber fiber);
        
        ActorFactoryConfigurator<TState> UseFiberFactory(FiberFactory fiberFactory);
        ActorFactoryConfigurator<TState> UseFiberFactory(FiberFactoryEx fiberFactory);

        ActorFactoryConfigurator<TState> SetExitTimeout(TimeSpan timeout);

        ActorFactoryConfigurator<TState> UseSharedScheduler();
        ActorFactoryConfigurator<TState> UseScheduler(Scheduler scheduler);
        ActorFactoryConfigurator<TState> UseSchedulerFactory(SchedulerFactory schedulerFactory);
    }
}