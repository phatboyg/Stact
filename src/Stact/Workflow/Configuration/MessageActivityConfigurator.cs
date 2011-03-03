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
	using System.Linq.Expressions;
	using Internal;
	using Magnum.Extensions;


	public class MessageActivityConfigurator<TWorkflow, TInstance, TBody> :
		ActivityConfigurator<TWorkflow, TInstance, TBody>,
		StateBuilderConfigurator<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		readonly IList<ActivityBuilderConfigurator<TWorkflow, TInstance, TBody>> _configurators;
		readonly Expression<Func<TWorkflow, Event<TBody>>> _eventExpression;
		readonly StateConfigurator<TWorkflow, TInstance> _stateConfigurator;

		public MessageActivityConfigurator(StateConfigurator<TWorkflow, TInstance> stateConfigurator,
		                                     Expression<Func<TWorkflow, Event<TBody>>> eventExpression)
		{
			_stateConfigurator = stateConfigurator;
			_eventExpression = eventExpression;

			_configurators = new List<ActivityBuilderConfigurator<TWorkflow, TInstance, TBody>>();
		}

		public void ValidateConfiguration()
		{
			if (_eventExpression == null)
				throw new StateMachineConfigurationException("Null event expression specified");
		}

		public void Configure(StateBuilder<TWorkflow, TInstance> builder)
		{
			MessageEvent<TBody> eevent = builder.Model.GetEvent(_eventExpression);

			if (builder.State == builder.Model.FinalState)
				throw new WorkflowDefinitionException("Events can not be specified for the final workflow state");

			var activityBuilder = new MessageActivityBuilder<TWorkflow, TInstance,TBody>(builder, eevent);

			_configurators.Each(x => x.Configure(activityBuilder));

			builder.AddActivity(activityBuilder.GetActivityExecutor());
		}

		public void AddConfigurator(StateBuilderConfigurator<TWorkflow, TInstance> configurator)
		{
			_stateConfigurator.AddConfigurator(configurator);
		}

		public void AddConfigurator(ActivityBuilderConfigurator<TWorkflow, TInstance, TBody> configurator)
		{
			_configurators.Add(configurator);
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