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
namespace Stact
{
	using System.Collections.Generic;
	using System.Linq;
	using Internal;
	using Magnum.Extensions;
	using Workflow;


	public class StateMachineWorkflowBinder<TWorkflow, TActor>
		where TActor : class, Actor
		where TWorkflow : class
	{
		readonly IDictionary<Event, StateMachineWorkflowEventBinder<TActor>> _binders;
		readonly StateMachineWorkflow<TWorkflow, TActor> _workflow;

		public StateMachineWorkflowBinder(StateMachineWorkflow<TWorkflow, TActor> workflow)
		{
			_workflow = workflow;

			_binders = BuildStateEventBinders(workflow);
		}

		static IDictionary<Event, StateMachineWorkflowEventBinder<TActor>> BuildStateEventBinders(
			StateMachineWorkflow<TWorkflow, TActor> workflow)
		{
			var visitor = new StateMachineWorkflowEventVisitor<TWorkflow, TActor>();
			workflow.Accept(visitor);

			return visitor.GetBinders().ToDictionary(x => x.Event);
		}

		public void Bind(Inbox inbox, TActor instance)
		{
			inbox.Loop(loop =>
				{
					_binders.Each(b => b.Value.Bind(loop, instance));
				});
		}
	}
}