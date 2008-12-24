// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.Common.StateMachine
{
	using System;
	using System.Collections.Generic;

	public class State<T> :
		State
		where T : StateMachine<T>
	{
		private readonly BasicEvent<T> _enter;
		private readonly BasicEvent<T> _leave;
		private readonly Dictionary<Event, Action<T, Event, object>> _actions;
		private readonly string _name;

		public State(string name)
		{
			_name = name;

			_enter = new BasicEvent<T>(string.Format("{0}:Enter", Name));
			_leave = new BasicEvent<T>(string.Format("{0}:Leave", Name));

			_actions = new Dictionary<Event, Action<T, Event, object>>();
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

		public void RaiseEvent(T instance, BasicEvent<T> eevent, object value)
		{
			Action<T, Event, object> action;
			if(_actions.TryGetValue(eevent, out action))
			{
				action(instance, eevent, value);
			}
		}

		public void EnterState(T instance)
		{
			RaiseEvent(instance, _enter, null);
		}

		public void LeaveState(T instance)
		{
			RaiseEvent(instance, _leave, null);
		}

		public override string ToString()
		{
			return string.Format("{0} (State)", _name);
		}

		public void BindEventAction(Event eevent, Action<T, Event, object> action)
		{
			if (_actions.ContainsKey(eevent))
				throw new StateMachineException(string.Format("The {0} event has already been added to the {1} state", eevent, _name));

			_actions.Add(eevent, action);
		}

		public static State<T> GetState(State input)
		{
			State<T> result = input as State<T>;
			if (result == null)
				throw new ArgumentException("The state is not valid for this state machine", "input");

			return result;
		}
	}

	public interface State
	{
		string Name { get; }

		Event Enter { get; }
		Event Leave { get; }
	}
}