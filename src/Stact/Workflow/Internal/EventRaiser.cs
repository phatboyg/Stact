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
	using Magnum.Extensions;
	using Magnum.Reflection;


	public class EventRaiser<TInstance>
	{
		readonly Event _event;
		Type _bodyType;
		Action<State<TInstance>, TInstance, object> _object;
		Action<State<TInstance>, TInstance> _simple;

		public EventRaiser(Event e)
		{
			_event = e;

			if (e.Implements(typeof(Event<>)))
				InitializeForMessageEvent();
			else
				InitializeForSimpleEvent();
		}

		public Event Event
		{
			get { return _event; }
		}

		void InitializeForSimpleEvent()
		{
			_simple = (state, instance) => state.RaiseEvent(instance, _event);

			_object = (state, instance, body) =>
				{
					//
					throw new StateMachineWorkflowException("Unexpected message for simple event: " + _event.Name);
				};
		}

		void InitializeForMessageEvent()
		{
			_bodyType = _event.GetType().GetGenericArguments()[0];

			_simple = (state, instance) =>
				{
					//
					throw new StateMachineWorkflowException("Message expected for event: " + _event.Name);
				};

			_object = (state, instance, body) =>
				{
					state.FastInvoke(new[] {_bodyType}, "RaiseEvent", instance, body);
				};
		}

		public void RaiseEvent(State<TInstance> state, TInstance instance)
		{
			_simple(state, instance);
		}

		public void RaiseEvent(State<TInstance> state, TInstance instance, object body)
		{
			_object(state, instance, body);
		}

		public void RaiseEvent<TBody>(State<TInstance> state, TInstance instance, TBody body)
		{
			if (typeof(TBody) != _bodyType)
				throw new StateMachineWorkflowException("Body type mismatch for message event: " + _event.Name);

			state.RaiseEvent(instance, (Event<TBody>)_event, body);
		}
	}
}