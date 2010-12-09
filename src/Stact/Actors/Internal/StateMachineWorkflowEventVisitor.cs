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
namespace Stact.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Workflow;
	using Workflow.Internal;


	public class StateMachineWorkflowEventVisitor<TWorkflow, TActor> :
		ReflectiveVisitorBase<StateMachineWorkflowEventVisitor<TWorkflow, TActor>>,
		StateMachineVisitor
		where TActor : class, Actor
		where TWorkflow : class
	{
		readonly IList<Activity<TActor>> _activities;
		readonly IDictionary<Event, StateMachineWorkflowEvent<TActor>> _events;
		StateMachineWorkflow<TWorkflow, TActor> _workflow;

		public StateMachineWorkflowEventVisitor()
		{
			_events = new Dictionary<Event, StateMachineWorkflowEvent<TActor>>();
			_activities = new List<Activity<TActor>>();
		}

		public IEnumerable<StateMachineWorkflowEventBinder<TActor>> GetBinders()
		{
				return _events
					.Select(x => x.Value.CreateBinder(_activities.Where(a => a.Event == x.Key).Select(a => a.State)));
		}

		protected virtual bool Visit<TBody>(MessageEvent<TBody> messageEvent)
		{
			_events.Retrieve(messageEvent, () => new StateMachineWorkflowEvent<TWorkflow, TActor, TBody>(_workflow, messageEvent));

			return true;
		}

		protected virtual bool Visit(Activity<TActor> activity)
		{
			_activities.Add(activity);

			return true;
		}

		protected virtual bool Visit(StateMachineWorkflow<TWorkflow, TActor> workflow) 
		{
			// this is the first thing hit, so let's make sure we are clean in case we get reused
			_activities.Clear();
			_events.Clear();

			_workflow = workflow;

			return true;
		}
	}
}