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
namespace Stact.Configuration.RegistryConfigurators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Stact.Internal;


	public class ActorRegistryConfiguratorImpl :
		FiberFactoryConfiguratorImpl<ActorRegistryConfigurator>,
		ActorRegistryConfigurator
	{
		readonly IList<RegistryBuilderConfigurator> _configurators;
		Func<Fiber, Scheduler, RegistryBuilder> _builderFactory;
		SchedulerFactory _schedulerFactory;

		public ActorRegistryConfiguratorImpl()
		{
			UseSharedScheduler();
			_builderFactory = DefaultBuilderFactory;
			_configurators = new List<RegistryBuilderConfigurator>();
		}

		public void ValidateConfiguration()
		{
			ValidateFiberFactoryConfiguration();
		}

		public ActorRegistryConfigurator UseSharedScheduler()
		{
			_schedulerFactory = () => new TimerScheduler(new PoolFiber());

			return this;
		}

		public ActorRegistryConfigurator UseScheduler(Scheduler scheduler)
		{
			_schedulerFactory = () => scheduler;

			return this;
		}

		public ActorRegistryConfigurator UseSchedulerFactory(SchedulerFactory schedulerFactory)
		{
			_schedulerFactory = schedulerFactory;

			return this;
		}

		public void UseBuilder(Func<Fiber, Scheduler, RegistryBuilder> builderFactory)
		{
			_builderFactory = builderFactory;
		}

		public void AddConfigurator(RegistryBuilderConfigurator configurator)
		{
			_configurators.Add(configurator);
		}

		static RegistryBuilder DefaultBuilderFactory(Fiber fiber, Scheduler scheduler)
		{
			return new InMemoryRegistryBuilder(fiber);
		}

		public ActorRegistry CreateRegistry()
		{
			FiberFactory fiberFactory = GetConfiguredFiberFactory();
			Fiber fiber = fiberFactory();

			Scheduler scheduler = _schedulerFactory();

			RegistryBuilder builder = _builderFactory(fiber, scheduler);

			return _configurators.
				Aggregate(builder, (current, configurator) => configurator.Configure(current))
				.Build();
		}
	}
}