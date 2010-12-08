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
	using System.Linq.Expressions;
	using System.Reflection;
	using Internal;


	public class MethodBodyConfigurator<TWorkflow, TInstance, TBody> :
		ActivityBuilderConfigurator<TWorkflow, TInstance, TBody>
		where TWorkflow : class
		where TInstance : class
	{
		readonly Expression<Func<TInstance, Action<TBody>>> _methodExpression;

		public MethodBodyConfigurator(Expression<Func<TInstance, Action<TBody>>> methodExpression)
		{
			_methodExpression = methodExpression;
		}

		public void ValidateConfigurator()
		{
			var me = _methodExpression.Body as MemberExpression;
			if (me == null || me.Member.MemberType != MemberTypes.Method)
				throw new StateMachineWorkflowConfiguratorException("Must be a method expression");

			var method = (MethodInfo)me.Member;
			if (!typeof(TInstance).IsAssignableFrom(method.DeclaringType))
				throw new StateMachineWorkflowConfiguratorException("Must be a method on the instance");
		}

		public void Configure(ActivityBuilder<TWorkflow, TInstance, TBody> builder)
		{
			var activity = new MethodBodyActivity<TInstance, TBody>(builder.State, builder.Event, _methodExpression);

			builder.AddActivity(activity);
		}
	}
}