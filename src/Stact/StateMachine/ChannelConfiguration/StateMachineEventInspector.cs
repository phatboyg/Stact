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
namespace Stact.StateMachine.ChannelConfiguration
{
	using System;
	using System.Collections.Generic;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Magnum.StateMachine;


	public class StateMachineEventInspector<TStateMachine> :
		ReflectiveVisitorBase<StateMachineEventInspector<TStateMachine>>,
		IStateMachineInspector
		where TStateMachine : StateMachine<TStateMachine>
	{
		readonly IDictionary<Event, StateMachineEvent<TStateMachine>> _events;
		State _currentState;

		public StateMachineEventInspector()
			: base("Inspect")
		{
			_events = new Dictionary<Event, StateMachineEvent<TStateMachine>>();
		}

		public void Inspect(object obj)
		{
			base.Visit(obj);
		}

		public void Inspect(object obj, Action action)
		{
			base.Visit(obj, () =>
				{
					action();
					return true;
				});
		}

		public bool Inspect<T>(T machine)
			where T : StateMachine<T>
		{
			return true;
		}

		public bool Inspect<T>(State<T> state)
			where T : StateMachine<T>
		{
			_currentState = state;

			return true;
		}

		public bool Inspect<T>(BasicEvent<T> eevent)
			where T : StateMachine<T>
		{
			return true;
		}

		public bool Inspect<T, V>(DataEvent<T, V> eevent)
			where T : StateMachine<T>
		{
			var target = eevent.CastAs<DataEvent<TStateMachine, V>>();

			Func<StateMachineEvent<TStateMachine>> factory = () => new StateMachineEvent<TStateMachine, V>(target, typeof(V));

			StateMachineEvent<TStateMachine> sagaEvent = _events.Retrieve(target, factory);

			sagaEvent.AddState(_currentState);

			return true;
		}

		public IEnumerable<StateMachineEventInspectorResult<TStateMachine>> GetResults()
		{
			foreach (var eventStates in _events)
				yield return eventStates.Value.ToResult();
		}


		public interface StateMachineEvent<T>
			where T : StateMachine<T>
		{
			void AddState(State state);
			StateMachineEventInspectorResult<T> ToResult();
		}


		public class StateMachineEvent<T, V> :
			StateMachineEvent<T>
			where T : StateMachine<T>
		{
			readonly IList<State> _states;

			public StateMachineEvent(DataEvent<T, V> eevent, Type messageType)
			{
				Event = eevent;
				MessageType = messageType;

				_states = new List<State>();
			}

			public DataEvent<T, V> Event { get; set; }
			public Type MessageType { get; set; }

			public void AddState(State state)
			{
				_states.Add(state);
			}

			public StateMachineEventInspectorResult<T> ToResult()
			{
				return new StateMachineEventInspectorResult<T, V>(Event, _states);
			}

			public bool Equals(StateMachineEvent<T, V> other)
			{
				if (ReferenceEquals(null, other))
					return false;
				if (ReferenceEquals(this, other))
					return true;
				return Equals(other.Event, Event) && Equals(other.MessageType, MessageType);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;
				if (ReferenceEquals(this, obj))
					return true;
				if (obj.GetType() != typeof(StateMachineEvent<T, V>))
					return false;
				return Equals((StateMachineEvent<T, V>)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((Event != null ? Event.GetHashCode() : 0)*397) ^ (MessageType != null ? MessageType.GetHashCode() : 0);
				}
			}
		}
	}
}