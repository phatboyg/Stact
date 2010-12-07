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


	public class DelegateStateEvent<TInstance> :
		StateEvent<TInstance>
		where TInstance : class
	{
		readonly Event _event;
		readonly Action<TInstance> _eventAction;
		readonly State<TInstance> _state;

		public DelegateStateEvent(State<TInstance> state, Event eevent, Action<TInstance> eventAction)
		{
			_event = eevent;
			_eventAction = eventAction;
			_state = state;
		}

		public State State
		{
			get { return _state; }
		}

		public Event Event
		{
			get { return _event; }
		}

		public void Accept(StateMachineVisitor visitor)
		{
			visitor.Visit(this);
		}

		public void Execute(TInstance instance)
		{
			_eventAction(instance);
		}

		public void Execute<T>(TInstance instance, T body)
		{
			_eventAction(instance);
		}
	}


	public class DelegateStateEvent<TInstance, TBody> :
		StateEvent<TInstance>
		where TInstance : class
	{
		readonly Event _event;
		readonly Action<TInstance, TBody> _eventAction;
		readonly State<TInstance> _state;

		public DelegateStateEvent(State<TInstance> state, Event eevent, Action<TInstance, TBody> eventAction)
		{
			_event = eevent;
			_eventAction = eventAction;
			_state = state;
		}

		public State State
		{
			get { return _state; }
		}

		public Event Event
		{
			get { return _event; }
		}

		public void Accept(StateMachineVisitor visitor)
		{
			visitor.Visit(this);
		}

		public void Execute(TInstance instance)
		{
			throw new StateMachineWorkflowException("Expected body on message was not present: " + _event.Name);
		}

		public void Execute<T>(TInstance instance, T body)
		{
			if (typeof(TBody) != typeof(T))
				throw new StateMachineWorkflowException("Body type mismatch for message event: " + _event.Name);

			_eventAction(instance, (TBody)((object)body));
		}
	}
}