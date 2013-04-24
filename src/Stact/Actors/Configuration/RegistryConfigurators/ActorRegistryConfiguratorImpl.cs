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
	using Internal;


	public class ActorRegistryConfiguratorImpl :
		SchedulerFactoryConfiguratorImpl<ActorRegistryConfigurator>,
		ActorRegistryConfigurator
	{
		readonly IList<RegistryBuilderConfigurator> _configurators;
		Func<Fiber, Scheduler, RegistryBuilder> _builderFactory;

		public ActorRegistryConfiguratorImpl()
		{
			UseTimerScheduler();

			_builderFactory = DefaultBuilderFactory;
			_configurators = new List<RegistryBuilderConfigurator>();
		}

		public void ValidateConfiguration()
		{
			ValidateFiberFactoryConfiguration();

		    foreach (var configurator in _configurators)
		    {
		        configurator.ValidateConfiguration();
		    }
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
			return new InMemoryRegistryBuilder(fiber, scheduler);
		}

		public ActorRegistry CreateRegistry()
		{
			ValidateConfiguration();

			FiberFactory fiberFactory = GetConfiguredFiberFactory();
			Fiber fiber = fiberFactory();

			Scheduler scheduler = GetConfiguredScheduler();

			RegistryBuilder builder = _builderFactory(fiber, scheduler);

			return _configurators
				.Aggregate(builder, (current, configurator) => configurator.Configure(current))
				.Build();
		}
	}
}