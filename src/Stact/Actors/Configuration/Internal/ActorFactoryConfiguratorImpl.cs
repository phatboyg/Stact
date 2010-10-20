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
namespace Stact.Configuration.Internal
{
	using System;
	using Stact.Internal;


	public class ActorFactoryConfiguratorImpl<TActor> :
		FiberFactoryConfiguratorImpl<ActorFactoryConfigurator<TActor>>,
		ActorFactoryConfigurator<TActor>
		where TActor : class, Actor
	{
		readonly ActorConventionSet<TActor> _conventions;
		Func<ActorFactory<TActor>> _actorFactory;
		SchedulerFactory _schedulerFactory;

		public ActorFactoryConfiguratorImpl()
		{
			_conventions = new ActorConventionSet<TActor>(new PropertyChannelsConvention<TActor>());

			UseSharedScheduler();
		}

		public ActorFactoryConfigurator<TActor> ConstructedBy(Func<TActor> actorFactory)
		{
			return ConstructedBy((f, s, i) => actorFactory());
		}

		public ActorFactoryConfigurator<TActor> ConstructedBy(Func<Inbox, TActor> actorFactory)
		{
			return ConstructedBy((f, s, i) => actorFactory(i));
		}

		public ActorFactoryConfigurator<TActor> ConstructedBy(Func<Fiber, TActor> actorFactory)
		{
			return ConstructedBy((f, s, i) => actorFactory(f));
		}

		public ActorFactoryConfigurator<TActor> ConstructedBy(Func<Fiber, Inbox, TActor> actorFactory)
		{
			return ConstructedBy((f, s, i) => actorFactory(f, i));
		}

		public ActorFactoryConfigurator<TActor> ConstructedBy(Func<Fiber, Scheduler, Inbox, TActor> actorFactory)
		{
			_actorFactory = () => new ActorFactoryImpl<TActor>(GetConfiguredFiberFactory(), _schedulerFactory, _conventions, actorFactory);

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

		public ActorFactoryConfigurator<TActor> AddConvention(ActorConvention<TActor> convention)
		{
			_conventions.Add(convention);

			return this;
		}

		public ActorFactory<TActor> CreateActorFactory()
		{
			return _actorFactory();
		}
	}
}