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


	public class StateConfiguratorImpl<TWorkflow, TInstance> :
		StateConfigurator<TWorkflow, TInstance>,
		StateMachineBuilderConfigurator<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		readonly IList<StateBuilderConfigurator<TWorkflow, TInstance>> _configurators;
		readonly Expression<Func<TWorkflow, State>> _stateExpression;
		readonly StateMachineConfigurator<TWorkflow, TInstance> _stateMachineConfigurator;

		public StateConfiguratorImpl(StateMachineConfigurator<TWorkflow, TInstance> stateMachineConfigurator,
		                             Expression<Func<TWorkflow, State>> stateExpression)
		{
			_stateMachineConfigurator = stateMachineConfigurator;
			_stateExpression = stateExpression;

			_configurators = new List<StateBuilderConfigurator<TWorkflow, TInstance>>();
		}

		public void AddConfigurator(StateBuilderConfigurator<TWorkflow, TInstance> configurator)
		{
			_configurators.Add(configurator);
		}

		public void Configure(StateMachineBuilder<TWorkflow, TInstance> stateMachineBuilder)
		{
			var state = stateMachineBuilder.GetState(_stateExpression);

			var stateBuilder = new StateBuilderImpl<TWorkflow, TInstance>(stateMachineBuilder, state);

			_configurators.Each(x => x.Configure(stateBuilder));
		}

		public void ValidateConfiguration()
		{
			if (_stateExpression == null)
				throw new StateMachineWorkflowConfiguratorException("Null state expression specified");
		}
	}
}