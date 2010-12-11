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
namespace Stact.Workflow
{
	using System;
	using System.Linq.Expressions;
	using Configuration;


	public static class ThenExtensions
	{
		public static ActivityConfigurator<TWorkflow, TInstance> Then<TWorkflow, TInstance>(
			this ActivityConfigurator<TWorkflow, TInstance> activityConfigurator, Action action)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new DelegateConfigurator<TWorkflow, TInstance>(action);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance, TBody> Then<TWorkflow, TInstance, TBody>(
			this ActivityConfigurator<TWorkflow, TInstance, TBody> activityConfigurator, Action action)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new DelegateConfigurator<TWorkflow, TInstance>(action);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static ExceptionConfigurator<TWorkflow, TInstance, TException> Then<TWorkflow, TInstance, TException>(
			this ExceptionConfigurator<TWorkflow, TInstance, TException> exceptionConfigurator, Action action)
			where TWorkflow : class
			where TInstance : class
			where TException : Exception
		{
			var configurator = new DelegateConfigurator<TWorkflow, TInstance>(action);

			exceptionConfigurator.AddConfigurator(configurator);

			return exceptionConfigurator;
		}

		public static MessageExceptionConfigurator<TWorkflow, TInstance, TException, TBody> Then
			<TWorkflow, TInstance, TException, TBody>(
			this MessageExceptionConfigurator<TWorkflow, TInstance, TException, TBody> exceptionConfigurator, Action action)
			where TWorkflow : class
			where TInstance : class
			where TException : Exception
		{
			var configurator = new DelegateConfigurator<TWorkflow, TInstance>(action);

			exceptionConfigurator.AddConfigurator(configurator);

			return exceptionConfigurator;
		}


		/// <summary>
		/// Invoke an action with the state machine instance
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="activityConfigurator"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance> Then<TWorkflow, TInstance>(
			this ActivityConfigurator<TWorkflow, TInstance> activityConfigurator, Action<TInstance> action)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new DelegateInstanceConfigurator<TWorkflow, TInstance>(action);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance, TBody> Then<TWorkflow, TInstance, TBody>(
			this ActivityConfigurator<TWorkflow, TInstance, TBody> activityConfigurator, Action<TInstance> action)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new DelegateInstanceConfigurator<TWorkflow, TInstance>(action);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static ExceptionConfigurator<TWorkflow, TInstance, TException> Then<TWorkflow, TInstance, TException>(
			this ExceptionConfigurator<TWorkflow, TInstance, TException> exceptionConfigurator, Action<TInstance> action)
			where TWorkflow : class
			where TInstance : class
			where TException : Exception
		{
			var configurator = new DelegateInstanceConfigurator<TWorkflow, TInstance>(action);

			exceptionConfigurator.AddConfigurator(configurator);

			return exceptionConfigurator;
		}

		public static MessageExceptionConfigurator<TWorkflow, TInstance, TException, TBody> Then
			<TWorkflow, TInstance, TException, TBody>(
			this MessageExceptionConfigurator<TWorkflow, TInstance, TException, TBody> exceptionConfigurator,
			Action<TInstance> action)
			where TWorkflow : class
			where TInstance : class
			where TException : Exception
		{
			var configurator = new DelegateInstanceConfigurator<TWorkflow, TInstance>(action);

			exceptionConfigurator.AddConfigurator(configurator);

			return exceptionConfigurator;
		}

		/// <summary>
		/// Invoke an action with the state machine instance and the body of the message event
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TBody"></typeparam>
		/// <param name="activityConfigurator"></param>
		/// <param name="eventAction"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance, TBody> Then<TWorkflow, TInstance, TBody>(
			this ActivityConfigurator<TWorkflow, TInstance, TBody> activityConfigurator, Action<TInstance, TBody> eventAction)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new DelegateInstanceBodyConfigurator<TWorkflow, TInstance, TBody>(eventAction);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static MessageExceptionConfigurator<TWorkflow, TInstance, TException, TBody> Then
			<TWorkflow, TInstance, TException, TBody>(
			this MessageExceptionConfigurator<TWorkflow, TInstance, TException, TBody> exceptionConfigurator,
			Action<TInstance, TBody> action)
			where TWorkflow : class
			where TInstance : class
			where TException : Exception
		{
			var configurator = new DelegateInstanceBodyConfigurator<TWorkflow, TInstance, TBody>(action);

			exceptionConfigurator.AddConfigurator(configurator);

			return exceptionConfigurator;
		}


		public static ActivityConfigurator<TWorkflow, TInstance, TBody> Then<TWorkflow, TInstance, TBody>(
			this ActivityConfigurator<TWorkflow, TInstance, TBody> activityConfigurator,
			Expression<Func<TInstance, Action>> methodExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new MethodConfigurator<TWorkflow, TInstance>(methodExpression);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance> Then<TWorkflow, TInstance>(
			this ActivityConfigurator<TWorkflow, TInstance> activityConfigurator,
			Expression<Func<TInstance, Action>> methodExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new MethodConfigurator<TWorkflow, TInstance>(methodExpression);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance, TBody> Then<TWorkflow, TInstance, TBody>(
			this ActivityConfigurator<TWorkflow, TInstance, TBody> activityConfigurator,
			Expression<Func<TInstance, Action<TBody>>> methodExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new MethodBodyConfigurator<TWorkflow, TInstance, TBody>(methodExpression);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}
	}
}