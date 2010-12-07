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
	using Internal;


	public class DelegateStateEventConfigurator<TWorkflow, TInstance> :
		StateEventBuilderConfigurator<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		readonly Action<TInstance> _eventAction;

		public DelegateStateEventConfigurator(Action<TInstance> eventAction)
		{
			_eventAction = eventAction;
		}

		public void ValidateConfigurator()
		{
		}

		public void Configure(StateEventBuilder<TWorkflow, TInstance> builder)
		{
			var stateEvent = new DelegateStateEvent<TInstance>(builder.State, builder.Event, _eventAction);

			builder.AddStateEvent(stateEvent);
		}
	}

	public class DelegateStateEventConfigurator<TWorkflow, TInstance, TBody> :
		StateEventBuilderConfigurator<TWorkflow, TInstance, TBody>
		where TWorkflow : class
		where TInstance : class
	{
		readonly Action<TInstance, TBody> _eventAction;

		public DelegateStateEventConfigurator(Action<TInstance, TBody> eventAction)
		{
			_eventAction = eventAction;
		}

		public void ValidateConfigurator()
		{
		}

		public void Configure(StateEventBuilder<TWorkflow, TInstance, TBody> builder)
		{
			var stateEvent = new DelegateStateEvent<TInstance, TBody>(builder.State, builder.Event, _eventAction);

			builder.AddStateEvent(stateEvent);
		}
	}
}