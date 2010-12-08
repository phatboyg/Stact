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


	public class StateMachineConfiguratorImpl<TWorkflow, TInstance> :
		StateMachineConfigurator<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		readonly IList<StateMachineBuilderConfigurator<TWorkflow, TInstance>> _configurators;
		Expression<Func<TInstance, State>> _currentStateExpression;

		public StateMachineConfiguratorImpl()
		{
			_configurators = new List<StateMachineBuilderConfigurator<TWorkflow, TInstance>>();
		}

		public void AddConfigurator(StateMachineBuilderConfigurator<TWorkflow, TInstance> configurator)
		{
			_configurators.Add(configurator);
		}

		public void AccessCurrentState(Expression<Func<TInstance, State>> stateExpression)
		{
			_currentStateExpression = stateExpression;
		}

		public StateMachineWorkflow<TWorkflow, TInstance> CreateStateMachineWorkflow()
		{
			ValidateConfigurators();

			var builder = new StateMachineBuilderImpl<TWorkflow, TInstance>(_currentStateExpression);

			foreach (var configurator in _configurators)
				configurator.Configure(builder);

			return builder.Build();
		}

		void ValidateConfigurators()
		{
			if (_currentStateExpression == null)
			{
				throw new StateMachineConfigurationException(
					"No accessor for the current state on the instance was specified");
			}


			foreach (var configurator in _configurators)
				configurator.ValidateConfiguration();
		}
	}
}