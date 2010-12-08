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
	using System.Linq.Expressions;


	public class MessageStateEventBuilder<TWorkflow, TInstance, TBody> :
		StateEventBuilder<TWorkflow, TInstance, TBody>
		where TWorkflow : class
		where TInstance : class
	{
		readonly StateBuilder<TWorkflow, TInstance> _builder;
		readonly MessageEvent<TBody> _event;

		public MessageStateEventBuilder(StateBuilder<TWorkflow, TInstance> builder, MessageEvent<TBody> eevent)
		{
			_builder = builder;
			_event = eevent;
		}

		public State<TInstance> State
		{
			get { return _builder.State; }
		}

		public StateMachineState<TInstance> GetState(string name)
		{
			return _builder.GetState(name);
		}

		public StateAccessor<TInstance> CurrentStateAccessor
		{
			get { return _builder.CurrentStateAccessor; }
		}

		public Event<TBody> Event
		{
			get { return _event; }
		}

		public SimpleEvent GetEvent(string name)
		{
			return _builder.GetEvent(name);
		}

		public SimpleEvent GetEvent(Expression<Func<TWorkflow, Event>> eventExpression)
		{
			return _builder.GetEvent(eventExpression);
		}

		public MessageEvent<T> GetEvent<T>(Expression<Func<TWorkflow, Event<T>>> eventExpression)
		{
			return _builder.GetEvent(eventExpression);
		}

		public StateMachineState<TInstance> GetState(Expression<Func<TWorkflow, State>> stateExpression)
		{
			return _builder.GetState(stateExpression);
		}

		public void AddStateEvent(StateEvent<TInstance> stateEvent)
		{
			_builder.AddStateEvent(stateEvent);
		}

		Event StateEventBuilder<TWorkflow, TInstance>.Event
		{
			get { return _event; }
		}
	}
}