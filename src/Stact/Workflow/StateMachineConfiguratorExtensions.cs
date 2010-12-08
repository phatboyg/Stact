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
	using Magnum.Extensions;


	public static class StateMachineConfiguratorExtensions
	{
		static void DoNothing<T>(T configurator)
		{
		}

		/// <summary>
		/// Selects a state to which events can be added
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="configurator"></param>
		/// <param name="stateExpression"></param>
		/// <returns></returns>
		public static StateConfigurator<TWorkflow, TInstance> During<TWorkflow, TInstance>(
			this StateMachineConfigurator<TWorkflow, TInstance> configurator,
			Expression<Func<TWorkflow, State>> stateExpression)
			where TWorkflow : class
			where TInstance : class
		{
			return configurator.During(stateExpression, DoNothing);
		}

		public static StateConfigurator<TWorkflow, TInstance> During<TWorkflow, TInstance>(
			this StateMachineConfigurator<TWorkflow, TInstance> configurator,
			Expression<Func<TWorkflow, State>> stateExpression,
			Action<StateConfigurator<TWorkflow, TInstance>> configurationAction)
			where TWorkflow : class
			where TInstance : class
		{
			var stateConfigurator = new StateConfiguratorImpl<TWorkflow, TInstance>(configurator, stateExpression);

			configurator.AddConfigurator(stateConfigurator);

			configurationAction(stateConfigurator);

			return stateConfigurator;
		}

		/// <summary>
		/// The initial state for a workflow (named Initial) to which events can be attached
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="configurator"></param>
		/// <returns></returns>
		public static StateConfigurator<TWorkflow, TInstance> Initially<TWorkflow, TInstance>(
			this StateMachineConfigurator<TWorkflow, TInstance> configurator)
			where TWorkflow : class
			where TInstance : class
		{
			return configurator.Initially(DoNothing);
		}

		public static StateConfigurator<TWorkflow, TInstance> Initially<TWorkflow, TInstance>(
			this StateMachineConfigurator<TWorkflow, TInstance> configurator,
			Action<StateConfigurator<TWorkflow, TInstance>> configurationAction)
			where TWorkflow : class
			where TInstance : class
		{
			var stateConfigurator = new StateConfiguratorImpl<TWorkflow, TInstance>(configurator,
			                                                                        StateMachineWorkflow.InitialStateName);

			configurator.AddConfigurator(stateConfigurator);

			configurationAction(stateConfigurator);

			return stateConfigurator;
		}

		/// <summary>
		/// Finally is triggered upon entry to the Final state
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="configurator"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance> Finally<TWorkflow, TInstance>(
			this StateMachineConfigurator<TWorkflow, TInstance> configurator)
			where TWorkflow : class
			where TInstance : class
		{
			return configurator.Finally(DoNothing);
		}

		/// <summary>
		/// Finally is triggered upon entry to the Final state
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="configurator"></param>
		/// <param name="configurationAction"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance> Finally<TWorkflow, TInstance>(
			this StateMachineConfigurator<TWorkflow, TInstance> configurator,
			Action<ActivityConfigurator<TWorkflow, TInstance>> configurationAction)
			where TWorkflow : class
			where TInstance : class
		{
			string finalName = StateMachineWorkflow.FinalStateName;

			var stateConfigurator = new StateConfiguratorImpl<TWorkflow, TInstance>(configurator, finalName);

			configurator.AddConfigurator(stateConfigurator);

			Expression<Func<State, Event>> selector = x => x.Entry;
			string finalEvent = finalName + "." + selector.MemberName();

			var eventConfigurator = new SimpleActivityConfigurator<TWorkflow, TInstance>(stateConfigurator, finalEvent);

			stateConfigurator.AddConfigurator(eventConfigurator);

			configurationAction(eventConfigurator);

			return eventConfigurator;
		}

		/// <summary>
		/// Selects an event that is accepted during the specified state
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="stateConfigurator"></param>
		/// <param name="eventExpression"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance> When<TWorkflow, TInstance>(
			this StateConfigurator<TWorkflow, TInstance> stateConfigurator, Expression<Func<TWorkflow, Event>> eventExpression)
			where TWorkflow : class
			where TInstance : class
		{
			return stateConfigurator.When(eventExpression, DoNothing);
		}

		public static ActivityConfigurator<TWorkflow, TInstance> When<TWorkflow, TInstance>(
			this StateConfigurator<TWorkflow, TInstance> stateConfigurator, Expression<Func<TWorkflow, Event>> eventExpression,
			Action<ActivityConfigurator<TWorkflow, TInstance>> configurationAction)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new SimpleActivityConfigurator<TWorkflow, TInstance>(stateConfigurator, eventExpression);

			stateConfigurator.AddConfigurator(configurator);

			configurationAction(configurator);

			return configurator;
		}


		/// <summary>
		/// Selects an event that is accepted during the specified state
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TBody"></typeparam>
		/// <param name="stateConfigurator"></param>
		/// <param name="eventExpression"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance, TBody> When<TWorkflow, TInstance, TBody>(
			this StateConfigurator<TWorkflow, TInstance> stateConfigurator,
			Expression<Func<TWorkflow, Event<TBody>>> eventExpression)
			where TWorkflow : class
			where TInstance : class
		{
			return stateConfigurator.When(eventExpression, DoNothing);
		}

		public static ActivityConfigurator<TWorkflow, TInstance, TBody> When<TWorkflow, TInstance, TBody>(
			this StateConfigurator<TWorkflow, TInstance> stateConfigurator,
			Expression<Func<TWorkflow, Event<TBody>>> eventExpression,
			Action<ActivityConfigurator<TWorkflow, TInstance, TBody>> configurationAction)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new MessageActivityConfigurator<TWorkflow, TInstance, TBody>(stateConfigurator, eventExpression);

			stateConfigurator.AddConfigurator(configurator);

			configurationAction(configurator);

			return configurator;
		}

		/// <summary>
		/// Transition the state machine to the specified state
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="configurator"></param>
		/// <param name="targetStateExpression"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance> TransitionTo<TWorkflow, TInstance>(
			this ActivityConfigurator<TWorkflow, TInstance> configurator,
			Expression<Func<TWorkflow, State>> targetStateExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var stateEventConfigurator = new TransitionConfigurator<TWorkflow, TInstance>(targetStateExpression);

			configurator.AddConfigurator(stateEventConfigurator);

			return configurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance, TBody> TransitionTo<TWorkflow, TInstance, TBody>(
			this ActivityConfigurator<TWorkflow, TInstance, TBody> configurator,
			Expression<Func<TWorkflow, State>> targetStateExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var stateEventConfigurator = new TransitionConfigurator<TWorkflow, TInstance>(targetStateExpression);

			configurator.AddConfigurator(stateEventConfigurator);

			return configurator;
		}

		/// <summary>
		/// Transition the state machine workflow to the Final state
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="configurator"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance> Finalize<TWorkflow, TInstance>(
			this ActivityConfigurator<TWorkflow, TInstance> configurator)
			where TWorkflow : class
			where TInstance : class
		{
			var stateEventConfigurator =
				new TransitionConfigurator<TWorkflow, TInstance>(StateMachineWorkflow.FinalStateName);

			configurator.AddConfigurator(stateEventConfigurator);

			return configurator;
		}

		/// <summary>
		/// Invoke an action with the state machine instance
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="stateEventConfigurator"></param>
		/// <param name="eventAction"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance> Then<TWorkflow, TInstance>(
			this ActivityConfigurator<TWorkflow, TInstance> stateEventConfigurator, Action<TInstance> eventAction)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new DelegateInstanceConfigurator<TWorkflow, TInstance>(eventAction);

			stateEventConfigurator.AddConfigurator(configurator);

			return stateEventConfigurator;
		}

		/// <summary>
		/// Invoke an action with the state machine instance and the body of the message event
		/// </summary>
		/// <typeparam name="TWorkflow"></typeparam>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TBody"></typeparam>
		/// <param name="stateEventConfigurator"></param>
		/// <param name="eventAction"></param>
		/// <returns></returns>
		public static ActivityConfigurator<TWorkflow, TInstance, TBody> Then<TWorkflow, TInstance, TBody>(
			this ActivityConfigurator<TWorkflow, TInstance, TBody> stateEventConfigurator, Action<TInstance, TBody> eventAction)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new DelegateInstanceBodyConfigurator<TWorkflow, TInstance, TBody>(eventAction);

			stateEventConfigurator.AddConfigurator(configurator);

			return stateEventConfigurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance, TBody> Then<TWorkflow, TInstance, TBody>(
			this ActivityConfigurator<TWorkflow, TInstance, TBody> stateEventConfigurator,
			Expression<Func<TInstance, Action<TBody>>> methodExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new MethodBodyConfigurator<TWorkflow, TInstance, TBody>(methodExpression);

			stateEventConfigurator.AddConfigurator(configurator);

			return stateEventConfigurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance, TBody> Then<TWorkflow, TInstance, TBody>(
			this ActivityConfigurator<TWorkflow, TInstance, TBody> stateEventConfigurator,
			Expression<Func<TInstance, Action>> methodExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new MethodConfigurator<TWorkflow, TInstance>(methodExpression);

			stateEventConfigurator.AddConfigurator(configurator);

			return stateEventConfigurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance> Then<TWorkflow, TInstance>(
			this ActivityConfigurator<TWorkflow, TInstance> stateEventConfigurator,
			Expression<Func<TInstance, Action>> methodExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new MethodConfigurator<TWorkflow, TInstance>(methodExpression);

			stateEventConfigurator.AddConfigurator(configurator);

			return stateEventConfigurator;
		}
	}
}