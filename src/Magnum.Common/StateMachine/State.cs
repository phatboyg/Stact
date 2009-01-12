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
		State,
		IStateMachineInspectorSite
		where T : StateMachine<T>
	{
		private readonly Dictionary<Event, List<StateEventAction<T>>> _actions;
		private readonly BasicEvent<T> _enter;
		private readonly BasicEvent<T> _leave;
		private readonly string _name;

		public State(string name)
		{
			_name = name;

			_enter = new BasicEvent<T>(string.Format("{0}:Enter", Name));
			_leave = new BasicEvent<T>(string.Format("{0}:Leave", Name));

			_actions = new Dictionary<Event, List<StateEventAction<T>>>();
		}

		public void Inspect(IStateMachineInspector inspector)
		{
			inspector.Inspect(this, () =>
				{
					foreach (var item in _actions)
					{
						var itemEvent = item.Key;
						var itemList = item.Value;

						inspector.Inspect(itemEvent, ()=>
							{
								foreach (var eventAction in itemList)
								{
									var action = eventAction;

									inspector.Inspect(action);
								}
							});
					}
				});
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
			List<StateEventAction<T>> eventActions;
			if (_actions.TryGetValue(eevent, out eventActions))
			{
				foreach (var action in eventActions)
				{
					action.Execute(instance, eevent, value);
				}
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

		public void BindEventAction(StateEventAction<T> action)
		{
			List<StateEventAction<T>> eventActions;
			if (_actions.TryGetValue(action.RaisedEvent, out eventActions) == false)
			{
				eventActions = new List<StateEventAction<T>> {action};

				_actions.Add(action.RaisedEvent, eventActions);
			}
			else
			{
				eventActions.Add(action);
			}
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