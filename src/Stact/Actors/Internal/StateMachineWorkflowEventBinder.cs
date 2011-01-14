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
namespace Stact.Internal
{
	using System;
	using System.Collections.Generic;
	using Workflow;


	public interface StateMachineWorkflowEventBinder<in TActor> :
		InboxReceiveBinder<TActor>
		where TActor : class, Actor
	{
		Type BodyType { get; }
		Event Event { get; }
		IEnumerable<State> ReceiveStates { get; }
	}


	public class StateMachineWorkflowEventBinder<TWorkflow, TActor, TBody> :
		StateMachineWorkflowEventBinder<TActor>
		where TActor : class, Actor
		where TWorkflow : class
	{
		readonly Event<TBody> _messageEvent;
		readonly HashSet<State> _receiveStates;
		readonly StateMachineWorkflow<TWorkflow, TActor> _workflow;

		public StateMachineWorkflowEventBinder(StateMachineWorkflow<TWorkflow, TActor> workflow, Event<TBody> messageEvent,
		                                       IEnumerable<State> receiveStates)
		{
			_receiveStates = new HashSet<State>(receiveStates);
			_messageEvent = messageEvent;
			_workflow = workflow;
		}

		public Type BodyType
		{
			get { return typeof(TBody); }
		}

		public Event Event
		{
			get { return _messageEvent; }
		}

		public IEnumerable<State> ReceiveStates
		{
			get { return _receiveStates; }
		}

		public PendingReceive Bind(Inbox inbox, TActor instance)
		{
			SelectiveConsumer<TBody> consumer = null;
			consumer = message =>
				{
					State currentState = _workflow.GetCurrentState(instance);

					if (!_receiveStates.Contains(currentState))
						return null;

					return m =>
						{
							try
							{
								_workflow.RaiseEvent(instance, _messageEvent, m);
							}
							finally
							{
								inbox.Receive(consumer);
							}
						};
				};

			return inbox.Receive(consumer);
		}
	}
}