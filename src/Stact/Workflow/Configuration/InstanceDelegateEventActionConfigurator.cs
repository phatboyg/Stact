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


	public class InstanceDelegateConfigurator<TWorkflow, TInstance> :
		ActivityBuilderConfigurator<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		readonly Action<TInstance> _action;

		public InstanceDelegateConfigurator(Action<TInstance> action)
		{
			_action = action;
		}

		public void ValidateConfigurator()
		{
			if (_action == null)
				throw new StateMachineWorkflowConfiguratorException("Null action specified");
		}

		public void Configure(ActivityBuilder<TWorkflow, TInstance> builder)
		{
			var activity = new DelegateInstanceActivity<TInstance>(builder.State, builder.Event, _action);

			builder.AddActivity(activity);
		}
	}
}