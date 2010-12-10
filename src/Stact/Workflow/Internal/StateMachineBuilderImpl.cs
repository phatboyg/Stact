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
		const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance;
		readonly string _anyStateName = StateMachineWorkflow.AnyStateName;
		readonly string _finalStateName = StateMachineWorkflow.FinalStateName;
		readonly string _initialStateName = StateMachineWorkflow.InitialStateName;
		readonly WorkflowModel<TWorkflow, TInstance> _model;

		public StateMachineBuilderImpl(Expression<Func<TInstance, State>> currentStateExpression)
		{
			var states = new Dictionary<string, State<TInstance>>(GetStates().ToDictionary(x => x.Name));
			var events = new Dictionary<string, Event>(GetEvents(states.Values).ToDictionary(x => x.Name));

			var anyState = states[_anyStateName];
			var initialState = states[_initialStateName];
			var finalState = states[_finalStateName];

			var currentState = new CurrentStateAccessor<TInstance>(currentStateExpression, initialState);

			_model = new WorkflowModelImpl<TWorkflow, TInstance>(states, events, currentState, anyState, initialState,finalState);
		}

		public WorkflowModel<TWorkflow, TInstance> Model
		{
			get { return _model; }
		}

		public StateMachineWorkflow<TWorkflow, TInstance> Build()
		{
			var workflow = new StateMachineWorkflowImpl<TWorkflow, TInstance>(_model);

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
				yield return state.Entry;
				yield return state.Exit;
			}
		}

		IEnumerable<State<TInstance>> GetStates()
		{
			IEnumerable<State<TInstance>> states = typeof(TWorkflow)
				.GetProperties(PropertyBindingFlags)
				.Where(property => property.PropertyType == typeof(State))
				.Select(property => new StateMachineState<TInstance>(property.Name))
				.Cast<State<TInstance>>();

			if (!states.Any(x => x.Name == _anyStateName))
			{
				states = states
					.Concat(Enumerable.Repeat<State<TInstance>>(new StateMachineState<TInstance>(_anyStateName), 1));
			}

			if (!states.Any(x => x.Name == _initialStateName))
			{
				states = states
					.Concat(Enumerable.Repeat<State<TInstance>>(new StateMachineState<TInstance>(_initialStateName), 1));
			}

			if (!states.Any(x => x.Name == _finalStateName))
			{
				states = states
					.Concat(Enumerable.Repeat<State<TInstance>>(new StateMachineState<TInstance>(_finalStateName), 1));
			}

			return states;
		}
	}
}