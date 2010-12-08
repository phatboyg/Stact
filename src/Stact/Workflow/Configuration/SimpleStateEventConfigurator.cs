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


	public class SimpleStateEventConfigurator<TWorkflow, TInstance> :
		StateEventConfigurator<TWorkflow, TInstance>,
		StateBuilderConfigurator<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		readonly StateConfigurator<TWorkflow, TInstance> _stateConfigurator;
		readonly IList<StateEventBuilderConfigurator<TWorkflow, TInstance>> _configurators;
		readonly Func<StateBuilder<TWorkflow, TInstance>, SimpleEvent> _getEvent;

		public SimpleStateEventConfigurator(StateConfigurator<TWorkflow, TInstance> stateConfigurator,
		                                    Expression<Func<TWorkflow, Event>> eventExpression)
			: this(stateConfigurator)
		{
			_getEvent = builder => builder.GetEvent(eventExpression);
		}

		public SimpleStateEventConfigurator(StateConfigurator<TWorkflow, TInstance> stateConfigurator,string eventName)
			: this(stateConfigurator)
		{
			_getEvent = builder => builder.GetEvent(eventName);
		}

		SimpleStateEventConfigurator(StateConfigurator<TWorkflow, TInstance> stateConfigurator)
		{
			_stateConfigurator = stateConfigurator;
			_configurators = new List<StateEventBuilderConfigurator<TWorkflow, TInstance>>();
		}

		public void ValidateConfiguration()
		{
			if (_getEvent == null)
				throw new StateMachineWorkflowConfiguratorException("Null event expression specified");
		}

		public void Configure(StateBuilder<TWorkflow, TInstance> builder)
		{
			SimpleEvent eevent = _getEvent(builder);

			var stateEventBuilder = new SimpleStateEventBuilder<TWorkflow, TInstance>(builder, eevent);

			_configurators.Each(x => x.Configure(stateEventBuilder));
		}

		public void AddConfigurator(StateBuilderConfigurator<TWorkflow, TInstance> configurator)
		{
			_stateConfigurator.AddConfigurator(configurator);
		}

		public void AddConfigurator(StateEventBuilderConfigurator<TWorkflow, TInstance> configurator)
		{
			_configurators.Add(configurator);
		}
	}
}