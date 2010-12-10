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


	public class MessageEventExceptionHandler<TInstance, TBody> :
		EventExceptionHandler<TInstance, TBody>
		where TInstance : class
	{
		readonly IList<EventExceptionHandler<TInstance, TBody>> _handlers;

		public MessageEventExceptionHandler()
		{
			_handlers = new List<EventExceptionHandler<TInstance, TBody>>();
		}

		public Type ExceptionType
		{
			get { return typeof(Exception); }
		}

		public ExceptionHandlerResult Handle(TInstance instance, Event eevent, TBody body, Exception exception)
		{
			Type exceptionType = exception.GetType();

			IEnumerable<EventExceptionHandler<TInstance, TBody>> matching = _handlers
				.Where(x => x.ExceptionType.IsAssignableFrom(exceptionType));

			foreach (var handler in matching)
			{
				ExceptionHandlerResult result = handler.Handle(instance, eevent, body, exception);
				if (result != ExceptionHandlerResult.Unhandled)
					return result;
			}

			return ExceptionHandlerResult.Unhandled;
		}

		public void Add(EventExceptionHandler<TInstance, TBody> exceptionHandler)
		{
			_handlers.Add(exceptionHandler);
		}

		public void Add(EventExceptionHandler<TInstance> exceptionHandler)
		{
			_handlers.Add(new ExceptionHandlerProxy(exceptionHandler));
		}


		class ExceptionHandlerProxy :
			EventExceptionHandler<TInstance, TBody>
		{
			readonly EventExceptionHandler<TInstance> _handler;

			public ExceptionHandlerProxy(EventExceptionHandler<TInstance> handler)
			{
				_handler = handler;
			}

			public Type ExceptionType
			{
				get { return _handler.ExceptionType; }
			}

			public ExceptionHandlerResult Handle(TInstance instance, Event eevent, TBody body, Exception exception)
			{
				return _handler.Handle(instance, eevent, exception);
			}
		}
	}
}