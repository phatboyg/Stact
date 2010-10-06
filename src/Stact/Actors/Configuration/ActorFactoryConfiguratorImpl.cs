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
	using Fibers;
	using Internal;


	public class ActorFactoryConfiguratorImpl<TActor> :
		FiberFactoryConfiguratorImpl<ActorFactoryConfigurator<TActor>>,
		ActorFactoryConfigurator<TActor>
		where TActor : class, Actor
	{
		Func<ActorFactory<TActor>> _actorFactory;
		SchedulerFactory _schedulerFactory;

		public ActorFactoryConfiguratorImpl()
		{
			UseSharedScheduler();
		}

		public ActorFactoryConfigurator<TActor> CreateNewInstanceBy(Func<Inbox, TActor> actorFactory)
		{
			_actorFactory =
				() =>
					{
						return new ActorFactoryImpl<TActor>(GetConfiguredFiberFactory(), _schedulerFactory,
						                                        (f, s, i) => { return actorFactory(i); });
					};

			return this;
		}

		public ActorFactoryConfigurator<TActor> CreateNewInstanceBy(Func<Fiber, TActor> actorFactory)
		{
			_actorFactory =
				() =>
					{
						return new ActorFactoryImpl<TActor>(GetConfiguredFiberFactory(), _schedulerFactory,
						                                        (f, s, i) => { return actorFactory(f); });
					};

			return this;
		}

		public ActorFactoryConfigurator<TActor> CreateNewInstanceBy(Func<Fiber, Inbox, TActor> actorFactory)
		{
			_actorFactory =
				() =>
					{
						return new ActorFactoryImpl<TActor>(GetConfiguredFiberFactory(), _schedulerFactory,
						                                        (f, s, i) => { return actorFactory(f, i); });
					};

			return this;
		}

		public ActorFactoryConfigurator<TActor> CreateNewInstanceBy(Func<Fiber, Scheduler, Inbox, TActor> actorFactory)
		{
			_actorFactory =
				() => { return new ActorFactoryImpl<TActor>(GetConfiguredFiberFactory(), _schedulerFactory, actorFactory); };

			return this;
		}

		public ActorFactoryConfigurator<TActor> UseSharedScheduler()
		{
			_schedulerFactory = () => new TimerScheduler(new PoolFiber());

			return this;
		}

		public ActorFactoryConfigurator<TActor> UseScheduler(Scheduler scheduler)
		{
			_schedulerFactory = () => scheduler;

			return this;
		}

		public ActorFactoryConfigurator<TActor> UseSchedulerFactory(SchedulerFactory schedulerFactory)
		{
			_schedulerFactory = schedulerFactory;

			return this;
		}

		public ActorFactory<TActor> CreateActorFactory()
		{
			return _actorFactory();
		}
	}
}