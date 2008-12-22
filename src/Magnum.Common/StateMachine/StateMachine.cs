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
	using System.Linq.Expressions;
	using System.Reflection;

	public class StateMachine<T> :
		StateMachine
		where T : StateMachine<T>
	{
		private static readonly HashSet<State<T>> _states = new HashSet<State<T>>();
		private static readonly HashSet<Event<T>> _events = new HashSet<Event<T>>();
		private static State<T> _initialState;

		static StateMachine()
		{
			InitializeStates();
			InitializeEvents();
		}

		protected StateMachine()
		{
			VerifyStateMachineConfiguration();

			_current = _initialState;
			_current.EnterState(this as T);
		}

		private State<T> _current;

		public State Current
		{
			get { return _current; }
		}

		public static void Define(Action definition)
		{
			definition();
		}

		protected static void When(State inputState, params StateEventAction<T>[] actions)
		{
			State<T> state = GetState(inputState);

			foreach (StateEventAction<T> action in actions)
			{
				state.BindEventAction(action.RaisedEvent, action.EventAction);
			}
		}

		private static State<T> GetState(State state)
		{
			State<T> stateOfT = state as State<T>;
			if (stateOfT == null)
				throw new ArgumentException("The state is not valid for this state machine", "state");

			return stateOfT;
		}

		private static Event<T> GetEvent(Event input)
		{
			Event<T> result = input as Event<T>;
			if (result == null)
				throw new ArgumentException("The state is not valid for this state machine", "input");

			return result;
		}


		protected static StateEventAction<T> OnEvent(Event raised, Action<T, Event<T>> action)
		{
			Event<T> raisedEvent = GetEvent(raised);

			var result = new StateEventAction<T>{RaisedEvent = raisedEvent, EventAction = action};

			return result;
		}

		protected static StateEventAction<T> OnEvent(Event raised, Action<T> action)
		{
			Event<T> raisedEvent = GetEvent(raised);

			var result = new StateEventAction<T>{RaisedEvent = raisedEvent, EventAction = (x,y) => action(x)};

			return result;
		}

		protected static Action<T, Event<T>> OnEntry(Action<T> action)
		{
			Action<T, Event<T>> result = (x, y) => action(x);

			return result;
		}

		public static void SetInitialState(State initialState)
		{
			_initialState = GetState(initialState);
		}

		protected void TransitionTo(State state)
		{
			_current.LeaveState(this as T);

			_current = GetState(state);

			_current.EnterState(this as T);
		}

		protected void RaiseEvent<V>(Event raised, V value)
		{
			Event<T> eevent = GetEvent(raised);

			_current.RaiseEvent(this as T, eevent);
		}

		protected static IEnumerable<State<T>> States
		{
			get { return _states; }
		}

		protected static IEnumerable<Event<T>> Events
		{
			get { return _events; }
		}

		private static void VerifyStateMachineConfiguration()
		{
			if (_states.Count == 0)
				throw new StateMachineException("A state machine must have at least one state to be valid.");

			if (_initialState == null)
				throw new StateMachineException("No initial state has been defined.");
		}

		private static void InitializeStates()
		{
			Type machineType = typeof (T);
			foreach (PropertyInfo propertyInfo in machineType.GetProperties(BindingFlags.Static | BindingFlags.Public))
			{
				if (!IsPropertyAState(propertyInfo)) continue;

				State<T> value = SetPropertyValue(propertyInfo, x => new State<T>(x.Name));

				_states.Add(value);
			}
		}	
		
		private static void InitializeEvents()
		{
			Type machineType = typeof (T);
			foreach (PropertyInfo propertyInfo in machineType.GetProperties(BindingFlags.Static | BindingFlags.Public))
			{
				if (!IsPropertyAnEvent(propertyInfo)) continue;

				Event<T> value = SetPropertyValue(propertyInfo, x => new Event<T>(x.Name));

				_events.Add(value);
			}
		}

		private static TValue SetPropertyValue<TValue>(PropertyInfo propertyInfo, Func<PropertyInfo, TValue> getValue)
		{
			var value = Expression.Parameter(typeof (TValue), "value");
			var action = Expression.Lambda<Action<TValue>>(Expression.Call(propertyInfo.GetSetMethod(), value), new[] {value}).Compile();

			TValue propertyValue = getValue(propertyInfo);
			action(propertyValue);

			return propertyValue;
		}

		private static bool IsPropertyAState(PropertyInfo propertyInfo)
		{
			return propertyInfo.PropertyType == typeof (State<T>) || propertyInfo.PropertyType == typeof (State);
		}

		private static bool IsPropertyAnEvent(PropertyInfo propertyInfo)
		{
			return propertyInfo.PropertyType == typeof (Event<T>) || propertyInfo.PropertyType == typeof (Event);
		}
	}

	public class StateMachineException : Exception
	{
		public StateMachineException(string message)
			: base(message)
		{
		}
	}

	public interface StateMachine
	{
		
	}
}