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


	public static class StateConfiguratorExtensions
	{
		public static StateEventConfigurator<TWorkflow, TInstance> When<TWorkflow, TInstance>(
			this StateConfigurator<TWorkflow, TInstance> stateConfigurator, Expression<Func<TWorkflow, Event>> eventExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator = new SimpleStateEventConfigurator<TWorkflow, TInstance>(stateConfigurator, eventExpression);

			stateConfigurator.AddConfigurator(configurator);

			return configurator;
		}

		public static StateEventConfigurator<TWorkflow, TInstance, TBody> When<TWorkflow, TInstance, TBody>(
			this StateConfigurator<TWorkflow, TInstance> stateConfigurator,
			Expression<Func<TWorkflow, Event<TBody>>> eventExpression) where TWorkflow : class where TInstance : class
		{
			var configurator = new MessageStateEventConfigurator<TWorkflow, TInstance, TBody>(stateConfigurator, eventExpression);

			stateConfigurator.AddConfigurator(configurator);

			return configurator;
		}
	}
}