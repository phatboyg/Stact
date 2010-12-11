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
	using Magnum.Extensions;


	public class ActivityEventExceptionHandler<TInstance, TException> :
		EventExceptionHandler<TInstance>
		where TInstance : class
		where TException : Exception
	{
		readonly IList<Activity<TInstance>> _activities;

		public ActivityEventExceptionHandler(IList<Activity<TInstance>> activities)
		{
			_activities = activities;
		}

		public Type ExceptionType
		{
			get { return typeof(TException); }
		}

		public ExceptionHandlerResult Handle(TInstance instance, Event eevent, Exception exception)
		{
			_activities.Each(x => x.Execute(instance));

			return ExceptionHandlerResult.Return;
		}
	}


	public class ActivityEventExceptionHandler<TInstance, TException, TBody> :
		EventExceptionHandler<TInstance, TBody>
		where TInstance : class
		where TException : Exception
	{
		readonly IList<Activity<TInstance>> _activities;

		public ActivityEventExceptionHandler(IList<Activity<TInstance>> activities)
		{
			_activities = activities;
		}

		public Type ExceptionType
		{
			get { return typeof(TException); }
		}

		public ExceptionHandlerResult Handle(TInstance instance, Event eevent, TBody body, Exception exception)
		{
			_activities.Each(x => x.Execute(instance, body));

			return ExceptionHandlerResult.Return;
		}
	}
}