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
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;


	public class StateMachineBuilderImpl<TWorkflow, TInstance> :
		StateMachineBuilder<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		public const string AnyStateName = "Any";
		public const string CompletedStateName = "Completed";
		public const string InitialStateName = "Initial";

		const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance;
		readonly State<TInstance> _anyState;
		readonly StateAccessor<TInstance> _currentState;
		readonly IDictionary<string, Event> _events;
		readonly State<TInstance> _initialState;
		readonly IDictionary<string, State<TInstance>> _states;

		public StateMachineBuilderImpl(Expression<Func<TInstance, State>> currentStateExpression)
			: this()
		{
			_currentState = new CurrentStateAccessor<TInstance>(currentStateExpression, _initialState);
		}

		StateMachineBuilderImpl()
		{
			_states = new Dictionary<string, State<TInstance>>(GetStates().ToDictionary(x => x.Name));

			_anyState = _states.Values.Where(x => x.Name == AnyStateName).Single();
			_initialState = _states.Values.Where(x => x.Name == InitialStateName).Single();

			_events = new Dictionary<string, Event>(GetEvents(_states.Values).ToDictionary(x => x.Name));
		}

		public StateMachineState<TInstance> GetState(Expression<Func<TWorkflow, State>> stateExpression)
		{
			return stateExpression.WithPropertyExpression(x => GetState(_states[x.Name]));
		}

		public StateMachineState<TInstance> GetState(string name)
		{
			return GetState(_states[name]);
		}

		public StateAccessor<TInstance> CurrentStateAccessor
		{
			get { return _currentState; }
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

		public StateMachineWorkflow<TWorkflow, TInstance> Build()
		{
			var workflow = new StateMachineWorkflowImpl<TWorkflow, TInstance>(_currentState, _states, _events.Values, _anyState);

			return workflow;
		}

		static IEnumerable<Event> GetEvents(IEnumerable<State<TInstance>> states)
		{
			foreach (PropertyInfo property in typeof(TWorkflow).GetProperties(PropertyBindingFlags))
			{
				if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
				{
					Type bodyType = property.PropertyType.GetGenericArguments()[0];
					Type eventType = typeof(MessageEvent<>).MakeGenericType(bodyType);

					var messageEvent = (Event)Activator.CreateInstance(eventType, property.Name);

					yield return messageEvent;
				}
				else if (property.PropertyType == typeof(Event))
				{
					var simpleEvent = new SimpleEvent(property.Name);

					yield return simpleEvent;
				}
			}

			foreach (var state in states)
			{
				yield return state.Enter;
				yield return state.Leave;
			}
		}

		static IEnumerable<State<TInstance>> GetStates()
		{
			IEnumerable<State<TInstance>> states = typeof(TWorkflow)
				.GetProperties(PropertyBindingFlags)
				.Where(property => property.PropertyType == typeof(State))
				.Select(property => new StateMachineState<TInstance>(property.Name))
				.Cast<State<TInstance>>();

			if (!states.Any(x => x.Name == AnyStateName))
				states = states.Concat(Enumerable.Repeat<State<TInstance>>(new StateMachineState<TInstance>(AnyStateName), 1));

			if (!states.Any(x => x.Name == InitialStateName))
				states = states.Concat(Enumerable.Repeat<State<TInstance>>(new StateMachineState<TInstance>(InitialStateName), 1));

			if (!states.Any(x => x.Name == CompletedStateName))
				states = states.Concat(Enumerable.Repeat<State<TInstance>>(new StateMachineState<TInstance>(CompletedStateName), 1));

			return states;
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