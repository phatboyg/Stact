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


	public class DelegateInstanceBodyActivity<TInstance, TBody> :
		ActivityBase<TInstance>
		where TInstance : class
	{
		readonly Action<TInstance, TBody> _action;

		public DelegateInstanceBodyActivity(State<TInstance> state, Event eevent, Action<TInstance, TBody> action)
			: base(state, eevent)
		{
			_action = action;
		}

		public override void Execute(TInstance instance)
		{
			throw new StateMachineWorkflowException("Expected body on message was not present: " + Event.Name);
		}

		public override void Execute<T>(TInstance instance, T body)
		{
			if (typeof(TBody) != typeof(T))
				throw new StateMachineWorkflowException("Body type mismatch for message event: " + Event.Name);

			_action(instance, (TBody)((object)body));
		}
	}
}