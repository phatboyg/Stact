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


	public static class StateMachineConfiguratorExtensions
	{
		public static StateConfigurator<TWorkflow, TInstance> During<TWorkflow, TInstance>(
			this StateMachineConfigurator<TWorkflow, TInstance> configurator,
			Expression<Func<TWorkflow, State>> stateExpression)
			where TWorkflow : class
			where TInstance : class
		{
			var stateConfigurator = new StateConfiguratorImpl<TWorkflow, TInstance>(configurator, stateExpression);

			configurator.AddConfigurator(stateConfigurator);

			return stateConfigurator;
		}
	}
}