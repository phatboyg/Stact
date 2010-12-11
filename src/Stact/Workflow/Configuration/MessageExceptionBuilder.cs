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
namespace Stact.Workflow.Configuration
{
	using System;
	using System.Collections.Generic;
	using Internal;


	public class MessageExceptionBuilder<TWorkflow, TInstance, TException, TBody> :
		ExceptionBuilder<TWorkflow, TInstance, TBody>
		where TInstance : class
		where TWorkflow : class
		where TException : Exception
	{
		readonly IList<Activity<TInstance>> _activities;
		readonly ActivityBuilder<TWorkflow, TInstance, TBody> _builder;

		public MessageExceptionBuilder(ActivityBuilder<TWorkflow, TInstance, TBody> builder)
		{
			_builder = builder;
			_activities = new List<Activity<TInstance>>();
		}

		public WorkflowModel<TWorkflow, TInstance> Model
		{
			get { return _builder.Model; }
		}

		public State<TInstance> State
		{
			get { return _builder.State; }
		}

		public void AddActivity(Activity<TInstance> activity)
		{
			_activities.Add(activity);
		}

		public Event Event
		{
			get { return _builder.Event; }
		}

		public void AddExceptionHandler(EventExceptionHandler<TInstance, TBody> exceptionHandler)
		{
			throw new NotImplementedException();
		}

		Event<TBody> ActivityBuilder<TWorkflow, TInstance, TBody>.Event
		{
			get { return _builder.Event; }
		}

		public void AddExceptionHandler(EventExceptionHandler<TInstance> exceptionHandler)
		{
			throw new InvalidOperationException("nested exception handlers are not supported");
		}

		public EventExceptionHandler<TInstance, TBody> CreateExceptionHandler()
		{
			return new ActivityEventExceptionHandler<TInstance, TException, TBody>(_activities);
		}
	}
}