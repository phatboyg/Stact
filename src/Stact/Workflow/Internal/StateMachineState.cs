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
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.Extensions;


	public class StateMachineState<TInstance> :
		State<TInstance>
		where TInstance : class
	{
		readonly SimpleEvent _enter;
		readonly IDictionary<Event, StateEventList<TInstance>> _events;
		readonly SimpleEvent _leave;
		readonly string _name;

		public StateMachineState(string name)
		{
			_name = name;

			_enter = new SimpleEvent(Name + ".Enter");
			_leave = new SimpleEvent(Name + ".Leave");

			_events = new Dictionary<Event, StateEventList<TInstance>>();
		}

		public void Accept(StateMachineVisitor visitor)
		{
			visitor.Visit(this);

			_events
				.OrderBy(x => x.Key)
				.Select(x => x.Value)
				.SelectMany(x => x)
				.Each(x => x.Accept(visitor));
		}

		public Event Enter
		{
			get { return _enter; }
		}

		public Event Leave
		{
			get { return _leave; }
		}

		public string Name
		{
			get { return _name; }
		}

		public void RaiseEvent(TInstance instance, Event eevent)
		{
			StateEventList<TInstance> stateEvent;
			if (_events.TryGetValue(eevent, out stateEvent))
				stateEvent.Execute(instance);
		}

		public void RaiseEvent<TBody>(TInstance instance, Event<TBody> eevent, TBody body)
		{
			StateEventList<TInstance> stateEvent;
			if (_events.TryGetValue(eevent, out stateEvent))
				stateEvent.Execute(instance, body);
		}

		public void AddStateEvent(StateEvent<TInstance> stateEvent)
		{
			StateEventList<TInstance> eventList = _events.Retrieve(stateEvent.Event, () => new StateEventList<TInstance>());

			eventList.Add(stateEvent);
		}

		public override string ToString()
		{
			return _name;
		}
	}
}