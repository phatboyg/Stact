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
namespace Stact.Workflow.Configuration
{
	using System;
	using System.Collections.Generic;
	using Internal;
	using Magnum.Extensions;


	public class MessageExceptionConfiguratorImpl<TWorkflow, TInstance, TBody> :
		MessageExceptionConfigurator<TWorkflow, TInstance, TBody>,
		ActivityBuilderConfigurator<TWorkflow, TInstance, TBody>
		where TWorkflow : class
		where TInstance : class
	{
		readonly ActivityConfigurator<TWorkflow, TInstance, TBody> _activityConfigurator;
		readonly List<ActivityBuilderConfigurator<TWorkflow, TInstance, TBody>> _configurators;

		public MessageExceptionConfiguratorImpl(ActivityConfigurator<TWorkflow, TInstance, TBody> activityConfigurator)
		{
			_activityConfigurator = activityConfigurator;

			_configurators = new List<ActivityBuilderConfigurator<TWorkflow, TInstance, TBody>>();
		}

		public void ValidateConfigurator()
		{
		}

		public void Configure(ActivityBuilder<TWorkflow, TInstance, TBody> builder)
		{
			_configurators.Each(x => x.Configure(builder));
		}

		public MessageExceptionConfigurator<TWorkflow, TInstance, TException, TBody> Exception<TException>()
			where TException : Exception
		{
			var configurator =
				new MessageExceptionConfiguratorImpl<TWorkflow, TInstance, TException, TBody>(_activityConfigurator);

			_configurators.Add(configurator);

			return configurator;
		}
	}


	public class MessageExceptionConfiguratorImpl<TWorkflow, TInstance, TException, TBody> :
		MessageExceptionConfigurator<TWorkflow, TInstance, TException, TBody>,
		ActivityBuilderConfigurator<TWorkflow, TInstance, TBody>
		where TWorkflow : class
		where TInstance : class
		where TException : Exception
	{
		readonly IList<ActivityBuilderConfigurator<TWorkflow, TInstance, TBody>> _configurators;
		readonly StateConfigurator<TWorkflow, TInstance> _stateConfigurator;

		public MessageExceptionConfiguratorImpl(StateConfigurator<TWorkflow, TInstance> stateConfigurator)
		{
			_stateConfigurator = stateConfigurator;

			_configurators = new List<ActivityBuilderConfigurator<TWorkflow, TInstance, TBody>>();
		}

		public void ValidateConfigurator()
		{
		}

		public void Configure(ActivityBuilder<TWorkflow, TInstance, TBody> builder)
		{
			var exceptionBuilder = new MessageExceptionBuilder<TWorkflow, TInstance, TException, TBody>(builder);

			ExtensionsToEnumerable.Each(_configurators, x => x.Configure(exceptionBuilder));

			builder.AddExceptionHandler(exceptionBuilder.CreateExceptionHandler());
		}

		public MessageExceptionConfigurator<TWorkflow, TInstance, T, TBody> Exception<T>()
			where T : Exception
		{
			var configurator = new MessageExceptionConfiguratorImpl<TWorkflow, TInstance, T, TBody>(_stateConfigurator);

			_configurators.Add(configurator);

			return configurator;
		}

		public void AddConfigurator(ActivityBuilderConfigurator<TWorkflow, TInstance, TBody> configurator)
		{
			_configurators.Add(configurator);
		}

		ExceptionConfigurator<TWorkflow, TInstance, T> ExceptionConfigurator<TWorkflow, TInstance>.Exception<T>()
		{
			var configurator = new ExceptionConfiguratorImpl<TWorkflow, TInstance, T>(_stateConfigurator);

			_configurators.Add(new ConfiguratorProxy(configurator));

			return configurator;
		}

		public void AddConfigurator(ActivityBuilderConfigurator<TWorkflow, TInstance> configurator)
		{
			_configurators.Add(new ConfiguratorProxy(configurator));
		}


		class ConfiguratorProxy :
			ActivityBuilderConfigurator<TWorkflow, TInstance, TBody>
		{
			readonly ActivityBuilderConfigurator<TWorkflow, TInstance> _configurator;

			public ConfiguratorProxy(ActivityBuilderConfigurator<TWorkflow, TInstance> configurator)
			{
				_configurator = configurator;
			}

			public void ValidateConfigurator()
			{
				_configurator.ValidateConfigurator();
			}

			public void Configure(ActivityBuilder<TWorkflow, TInstance, TBody> builder)
			{
				_configurator.Configure(builder);
			}
		}
	}
}