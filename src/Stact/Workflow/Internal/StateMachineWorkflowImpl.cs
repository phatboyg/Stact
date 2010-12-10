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
	using Magnum.Extensions;


	public class StateMachineWorkflowImpl<TWorkflow, TInstance> :
		StateMachineWorkflow<TWorkflow, TInstance>
		where TWorkflow : class
		where TInstance : class
	{
		readonly IDictionary<string, EventRaiser<TInstance>> _events;
		readonly IDictionary<Event, EventRaiser<TInstance>> _eventsByKey;
		readonly WorkflowModel<TWorkflow, TInstance> _model;

		public StateMachineWorkflowImpl(WorkflowModel<TWorkflow,TInstance> model)
		{
			_model = model;
			_events = model.Events.Select(x => new EventRaiser<TInstance>(x)).ToDictionary(x => x.Event.Name);
			_eventsByKey = _events.Values.ToDictionary(x => x.Event);
		}

		public void Accept(StateMachineVisitor visitor)
		{
			visitor.Visit(this);

			_model.States.Each(x => x.Accept(visitor));
			_model.Events.Each(x => x.Accept(visitor));
		}

		public void RaiseEvent(TInstance instance, string name)
		{
			EventRaiser<TInstance> e;
			if (!_events.TryGetValue(name, out e))
				throw new UnknownEventException(typeof(TWorkflow), name);

			State<TInstance> state = _model.CurrentStateAccessor.Get(instance);

			e.RaiseEvent(state, instance);
		}

		public void RaiseEvent(TInstance instance, string name, object body)
		{
			EventRaiser<TInstance> e;
			if (!_events.TryGetValue(name, out e))
				throw new UnknownEventException(typeof(TWorkflow), name);

			State<TInstance> state = _model.CurrentStateAccessor.Get(instance);

			e.RaiseEvent(state, instance, body);
		}

		public void RaiseEvent(TInstance instance, Event eevent)
		{
			EventRaiser<TInstance> e;
			if (!_eventsByKey.TryGetValue(eevent, out e))
				throw new UnknownEventException(typeof(TWorkflow), eevent.Name);

			State<TInstance> state = _model.CurrentStateAccessor.Get(instance);

			e.RaiseEvent(state, instance);
			e.RaiseEvent(_model.AnyState, instance);
		}

		public void RaiseEvent<TBody>(TInstance instance, Event<TBody> eevent, TBody body)
		{
			EventRaiser<TInstance> e;
			if (!_eventsByKey.TryGetValue(eevent, out e))
				throw new UnknownEventException(typeof(TWorkflow), eevent.Name);

			State<TInstance> state = _model.CurrentStateAccessor.Get(instance);

			e.RaiseEvent(state, instance, body);
			e.RaiseEvent(_model.AnyState, instance, body);
		}

		public void RaiseEvent(TInstance instance, Expression<Func<TWorkflow, Event>> eventSelector)
		{
			EventRaiser<TInstance> e;
			if (!_events.TryGetValue(eventSelector.GetEventName(), out e))
				throw new UnknownEventException(typeof(TWorkflow), eventSelector.GetEventName());

			State<TInstance> state = _model.CurrentStateAccessor.Get(instance);

			e.RaiseEvent(state, instance);
			e.RaiseEvent(_model.AnyState, instance);
		}

		public void RaiseEvent<TBody>(TInstance instance, Expression<Func<TWorkflow, Event<TBody>>> eventSelector, TBody body)
		{
			EventRaiser<TInstance> e;
			if (!_events.TryGetValue(eventSelector.GetEventName(), out e))
				throw new UnknownEventException(typeof(TWorkflow), eventSelector.GetEventName());

			State<TInstance> state = _model.CurrentStateAccessor.Get(instance);

			e.RaiseEvent(state, instance, body);
			e.RaiseEvent(_model.AnyState, instance, body);
		}

		public State GetCurrentState(TInstance instance)
		{
			return _model.CurrentStateAccessor.Get(instance);
		}

		public State GetState(string name)
		{
			return _model.GetState(name);
		}

		public Event GetEvent(string name)
		{
			return _model.GetEvent(name);
		}

		public Event GetEvent(Expression<Func<TWorkflow, Event>> eventExpression)
		{
			return _model.GetEvent(eventExpression);
		}

		public Event<TBody> GetEvent<TBody>(Expression<Func<TWorkflow, Event<TBody>>> eventExpression)
		{
			return _model.GetEvent(eventExpression);
		}

		public State GetState(Expression<Func<TWorkflow, State>> stateExpression)
		{
			return _model.GetState(stateExpression);
		}
	}
}