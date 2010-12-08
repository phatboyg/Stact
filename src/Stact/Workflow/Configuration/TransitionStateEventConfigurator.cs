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


	public class TransitionStateEventConfigurator<TWorkflow, TInstance> :
		StateEventBuilderConfigurator<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		Func<StateEventBuilder<TWorkflow, TInstance>, StateMachineState<TInstance>> _getState;

		public TransitionStateEventConfigurator(Expression<Func<TWorkflow, State>> stateExpression)
		{
			_getState = builder => builder.GetState(stateExpression);
		}

		public TransitionStateEventConfigurator(string stateName)
		{
			_getState = builder => builder.GetState(stateName);
		}

		public void ValidateConfigurator()
		{
		}

		public void Configure(StateEventBuilder<TWorkflow, TInstance> builder)
		{
			StateMachineState<TInstance> state = _getState(builder);

			var stateEvent = new TransitionStateEvent<TInstance>(builder.CurrentStateAccessor, builder.State, builder.Event,
			                                                     state);

			builder.AddStateEvent(stateEvent);
		}
	}
}