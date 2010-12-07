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
	using System.Reflection;


	public static class ExpressionUtil
	{
		public static TResult WithPropertyExpression<T, TProperty, TResult>(
			this Expression<Func<T, TProperty>> propertyExpression,
			Func<PropertyInfo, TResult> transformer)
		{
			var me = propertyExpression.Body as MemberExpression;
			if (me == null || me.Member.MemberType != MemberTypes.Property)
				throw new ArgumentException("The expression must be a property expression");

			var property = (PropertyInfo)me.Member;

			return transformer(property);
		}

		public static string GetEventName<TWorkflow, TBody>(this Expression<Func<TWorkflow, Event<TBody>>> eventExpression)
		{
			return eventExpression.WithPropertyExpression(x =>
				{
					if (x.DeclaringType != typeof(TWorkflow))
						throw new StateMachineWorkflowException("Expression must be a property of the workflow interface");

					return x.Name;
				});
		}

		public static string GetEventName<TWorkflow>(this Expression<Func<TWorkflow, Event>> eventExpression)
		{
			return eventExpression.WithPropertyExpression(x =>
				{
					if (x.DeclaringType != typeof(TWorkflow))
						throw new StateMachineWorkflowException("Expression must be a property of the workflow interface");

					return x.Name;
				});
		}

		public static string GetStateName<TWorkflow>(this Expression<Func<TWorkflow, State>> eventExpression)
		{
			return eventExpression.WithPropertyExpression(x =>
				{
					if (x.DeclaringType != typeof(TWorkflow))
						throw new StateMachineWorkflowException("Expression must be a property of the workflow interface");

					return x.Name;
				});
		}
	}
}