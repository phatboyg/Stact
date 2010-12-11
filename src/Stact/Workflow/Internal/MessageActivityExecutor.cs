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


	public class MessageActivityExecutor<TInstance, TBody> :
		Activity<TInstance>
		where TInstance : class
	{
		readonly ActivityList<TInstance> _activities;
		readonly Event _event;
		readonly EventExceptionHandler<TInstance, TBody> _exceptionHandler;
		readonly State _state;

		public MessageActivityExecutor(State state, Event eevent, EventExceptionHandler<TInstance, TBody> exceptionHandler)
		{
			_exceptionHandler = exceptionHandler;
			_state = state;
			_event = eevent;
			_activities = new ActivityList<TInstance>();
		}

		public void Execute(TInstance instance)
		{
			try
			{
				_activities.Execute(instance);
			}
			catch (Exception innerException)
			{
				string message = "{0} occurred while handling {1} during {2}"
					.FormatWith(innerException.GetType().ToShortTypeName(), _event.Name, _state.Name);

				throw new StateMachineWorkflowException(message, innerException);
			}
		}

		public void Execute<T>(TInstance instance, T bodyT)
		{
			if (typeof(TBody) != typeof(T))
				throw new StateMachineWorkflowException("Body type mismatch for message event: " + Event.Name);

			var body = (TBody)((object)bodyT);

			while (true)
			{
				try
				{
					_activities.Execute(instance, body);
					return;
				}
				catch (Exception ex)
				{
					try
					{
						ExceptionHandlerResult result = _exceptionHandler.Handle(instance, _event, body, ex);
						if (result == ExceptionHandlerResult.Unhandled)
							throw;

						if (result == ExceptionHandlerResult.Return)
							return;
					}
					catch (Exception innerException)
					{
						string message = "{0} occurred while handling {1} during {2}"
							.FormatWith(innerException.GetType().ToShortTypeName(), _event.Name, _state.Name);

						throw new StateMachineWorkflowException(message, innerException);
					}
				}
			}
		}

		public void Accept(StateMachineVisitor visitor)
		{
			_activities.Each(x => x.Accept(visitor));
		}

		public State State
		{
			get { return _state; }
		}

		public Event Event
		{
			get { return _event; }
		}

		public void Add(Activity<TInstance> activity)
		{
			_activities.Add(activity);
		}
	}
}