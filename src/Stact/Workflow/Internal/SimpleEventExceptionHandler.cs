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


	public class SimpleEventExceptionHandler<TInstance> :
		EventExceptionHandler<TInstance>
		where TInstance : class
	{
		readonly IList<EventExceptionHandler<TInstance>> _handlers;

		public SimpleEventExceptionHandler()
		{
			_handlers = new List<EventExceptionHandler<TInstance>>();
		}

		public Type ExceptionType
		{
			get { return typeof(Exception); }
		}

		public ExceptionHandlerResult Handle(TInstance instance, Event eevent, Exception exception)
		{
			Type exceptionType = exception.GetType();

			IEnumerable<EventExceptionHandler<TInstance>> matching = _handlers
				.Where(x => x.ExceptionType.IsAssignableFrom(exceptionType));

			var finalResult = ExceptionHandlerResult.Unhandled;

			foreach (var handler in matching)
			{
				ExceptionHandlerResult result = handler.Handle(instance, eevent, exception);
				if (result == ExceptionHandlerResult.Return)
				{
					finalResult = ExceptionHandlerResult.Return;
					continue;
				}

				if (result != ExceptionHandlerResult.Unhandled)
					return result;
			}

			return finalResult;
		}

		public void Add(EventExceptionHandler<TInstance> exceptionHandler)
		{
			_handlers.Add(exceptionHandler);
		}
	}
}