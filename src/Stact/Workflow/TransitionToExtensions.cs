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


	public static class TransitionToExtensions
	{
		/// <summary>
		/// Transition the state machine to the specified state
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="activityConfigurator"></param>
		/// <param name="targetStateExpression"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance> TransitionTo<TWorkflow, TInstance>(
			this ActivityConfigurator<TWorkflow, TInstance> activityConfigurator,
			Expression<Func<TWorkflow, State>> targetStateExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new TransitionConfigurator<TWorkflow, TInstance>(targetStateExpression);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static ExceptionConfigurator<TWorkflow, TInstance, TException> TransitionTo<TWorkflow, TInstance, TException>(
			this ExceptionConfigurator<TWorkflow, TInstance, TException> activityConfigurator,
			Expression<Func<TWorkflow, State>> targetStateExpression)
			where TWorkflow : class
			where TInstance : class
			where TException : Exception
		{
			var configurator = new TransitionConfigurator<TWorkflow, TInstance>(targetStateExpression);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance, TBody> TransitionTo<TWorkflow, TInstance, TBody>(
			this ActivityConfigurator<TWorkflow, TInstance, TBody> activityConfigurator,
			Expression<Func<TWorkflow, State>> targetStateExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new TransitionConfigurator<TWorkflow, TInstance>(targetStateExpression);

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}
	}
}