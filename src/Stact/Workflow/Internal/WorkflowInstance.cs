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
namespace Stact.Workflow.Internal
{
	using System;
	using System.Linq.Expressions;


	public class WorkflowInstance<TWorkflow, TInstance> :
		WorkflowInstance<TWorkflow>
		where TWorkflow : class
		where TInstance : class
	{
		readonly TInstance _instance;
		readonly StateMachineWorkflow<TWorkflow, TInstance> _workflow;

		public WorkflowInstance(StateMachineWorkflow<TWorkflow, TInstance> workflow, TInstance instance)
		{
			_workflow = workflow;
			_instance = instance;
		}

		public void RaiseEvent(string eventName)
		{
			_workflow.RaiseEvent(_instance, eventName);
		}

		public void RaiseEvent(string eventName, object body)
		{
			_workflow.RaiseEvent(_instance, eventName, body);
		}

		public State CurrentState
		{
			get { return _workflow.GetCurrentState(_instance); }
		}

		public void RaiseEvent(Expression<Func<TWorkflow, Event>> eventSelector)
		{
			_workflow.RaiseEvent(_instance, eventSelector);
		}

		public void RaiseEvent<TBody>(Expression<Func<TWorkflow, Event<TBody>>> eventSelector, TBody body)
		{
			_workflow.RaiseEvent(_instance, eventSelector, body);
		}
	}
}