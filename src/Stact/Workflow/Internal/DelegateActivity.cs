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


	public class DelegateActivity<TInstance> :
		ActivityBase<TInstance>
		where TInstance : class
	{
		readonly Action _action;

		public DelegateActivity(State<TInstance> state, Event eevent, Action action)
			: base(state, eevent)
		{
			_action = action;
		}

		public override void Execute(TInstance instance)
		{
			_action();
		}

		public override void Execute<T>(TInstance instance, T body)
		{
			_action();
		}
	}
}