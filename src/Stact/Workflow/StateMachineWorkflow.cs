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
namespace Stact.Workflow
{
	using System;
	using System.Linq.Expressions;
	using Configuration;


	public static class StateMachineWorkflow
	{
		public static StateMachineWorkflow<TWorkflow, TInstance> New<TWorkflow, TInstance>(
			Action<StateMachineConfigurator<TWorkflow, TInstance>> configure)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new StateMachineConfiguratorImpl<TWorkflow, TInstance>();

			configure(configurator);

			return configurator.CreateStateMachineWorkflow();
		}
	}


	/// <summary>
	/// A state machine workflow encapsulates the definition of a state machine behind a 
	/// workflow interface. The interface can be used to apply the state machine behavior
	/// to a class instance. It can also be visited to graph the state machineb behavior 
	/// for user review.
	/// </summary>
	/// <typeparam name="TWorkflow">The interface that defines the states and events for the workflow</typeparam>
	/// <typeparam name="TInstance">The instance that is used to track the state</typeparam>
	public interface StateMachineWorkflow<TWorkflow, in TInstance> :
		WorkflowDefinition<TWorkflow>,
		AcceptStateMachineVisitor
		where TWorkflow : class
		where TInstance : class
	{
		void RaiseEvent(TInstance instance, string eventName);
		void RaiseEvent(TInstance instance, string eventName, object body);

		void RaiseEvent(TInstance instance, Event eevent);
		void RaiseEvent<TBody>(TInstance instance, Event<TBody> eevent, TBody body);

		void RaiseEvent(TInstance instance, Expression<Func<TWorkflow, Event>> eventSelector);
		void RaiseEvent<TBody>(TInstance instance, Expression<Func<TWorkflow, Event<TBody>>> eventSelector, TBody body);
		
		State GetCurrentState(TInstance instance);
	}
}