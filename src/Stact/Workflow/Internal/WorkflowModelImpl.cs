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
	using System.Collections.Generic;
	using System.Linq.Expressions;


	public class WorkflowModelImpl<TWorkflow, TInstance> :
		WorkflowModel<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		readonly StateAccessor<TInstance> _currentState;
		readonly IDictionary<string, Event> _events;
		readonly IDictionary<string, State<TInstance>> _states;

		public WorkflowModelImpl(IDictionary<string, State<TInstance>> states, IDictionary<string, Event> events,
		                         StateAccessor<TInstance> currentStateAccessor, 
		                         State<TInstance> initialState, State<TInstance> finalState)
		{
			InitialState = initialState;
			FinalState = finalState;
			_states = states;
			_events = events;
			_currentState = currentStateAccessor;
		}

		public State<TInstance> InitialState { get; private set; }
		public State<TInstance> FinalState { get; private set; }

		public StateAccessor<TInstance> CurrentStateAccessor
		{
			get { return _currentState; }
		}

		public IEnumerable<Event> Events
		{
			get { return _events.Values; }
		}

		public IEnumerable<State<TInstance>> States
		{
			get { return _states.Values; }
		}


		public StateMachineState<TInstance> GetState(Expression<Func<TWorkflow, State>> stateExpression)
		{
			return stateExpression.WithPropertyExpression(x => GetState(_states[x.Name]));
		}

		public StateMachineState<TInstance> GetState(string name)
		{
			return GetState(_states[name]);
		}

		public SimpleEvent GetEvent(string name)
		{
			return GetEvent(_events[name]);
		}

		public SimpleEvent GetEvent(Expression<Func<TWorkflow, Event>> eventExpression)
		{
			return eventExpression.WithPropertyExpression(x => GetEvent(_events[x.Name]));
		}

		public MessageEvent<TBody> GetEvent<TBody>(Expression<Func<TWorkflow, Event<TBody>>> eventExpression)
		{
			return eventExpression.WithPropertyExpression(x => GetEvent<TBody>(_events[x.Name]));
		}

		static StateMachineState<TInstance> GetState(State input)
		{
			var result = input as StateMachineState<TInstance>;
			if (result == null)
				throw new UnknownStateException(typeof(TWorkflow), input.Name);

			return result;
		}

		static SimpleEvent GetEvent(Event input)
		{
			var result = input as SimpleEvent;
			if (result == null)
				throw new UnknownEventException(typeof(TWorkflow), input.Name);

			return result;
		}

		static MessageEvent<TBody> GetEvent<TBody>(Event input)
		{
			var result = input as MessageEvent<TBody>;
			if (result == null)
				throw new UnknownEventException(typeof(TWorkflow), input.Name);

			return result;
		}
	}
}