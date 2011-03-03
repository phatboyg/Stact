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
	using System.Linq.Expressions;
	using Internal;


	public class TransitionConfigurator<TWorkflow, TInstance> :
		ActivityBuilderConfigurator<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		readonly Func<WorkflowModel<TWorkflow, TInstance>, StateMachineState<TInstance>> _getTargetState;
		readonly string _stateName;

		public TransitionConfigurator(Expression<Func<TWorkflow, State>> stateExpression)
		{
			_getTargetState = m => m.GetState(stateExpression);
			_stateName = stateExpression.GetStateName();
		}

		public TransitionConfigurator(string stateName)
		{
			_getTargetState = m => m.GetState(stateName);
			_stateName = stateName;
		}

		public void ValidateConfigurator()
		{
			if (_stateName == StateMachineWorkflow.InitialStateName)
				throw new WorkflowDefinitionException("A transition to the initial state is not allowed.");
		}

		public void Configure(ActivityBuilder<TWorkflow, TInstance> builder)
		{
			StateMachineState<TInstance> targetState = _getTargetState(builder.Model);

			var activity = new TransitionActivity<TInstance>(builder.Model.CurrentStateAccessor, builder.State, builder.Event,
			                                                 targetState);

			builder.AddActivity(activity);
		}
	}
}