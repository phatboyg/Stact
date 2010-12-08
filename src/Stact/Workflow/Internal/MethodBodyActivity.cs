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
	using System.Linq.Expressions;
	using System.Reflection;
	using Configuration;


	public class MethodBodyActivity<TInstance, TBody> :
		ActivityBase<TInstance>
		where TInstance : class
	{
		readonly Action<TInstance, TBody> _method;
		readonly MethodInfo _methodInfo;

		public MethodBodyActivity(State<TInstance> state, Event eevent,Expression<Func<TInstance, Action<TBody>>> methodExpression)
			: base(state, eevent)
		{
			_methodInfo = new FindMethodCallVisitor().Find(methodExpression);
			if (_methodInfo == null)
				throw new StateMachineConfigurationException("The expression method info could not be found: "
				                                                    + methodExpression);

			_method = CompileMethod(_methodInfo);
		}

		public string MethodName
		{
			get { return _methodInfo.Name; }
		}

		public override void Execute(TInstance instance)
		{
			throw new StateMachineWorkflowException("Expected body on message was not present: " + Event.Name);
		}

		public override void Execute<T>(TInstance instance, T body)
		{
			if (typeof(TBody) != typeof(T))
				throw new StateMachineWorkflowException("Body type mismatch for message event: " + Event.Name);

			_method(instance, (TBody)((object)body));
		}

		static Action<TInstance, TBody> CompileMethod(MethodInfo methodInfo)
		{
			ParameterExpression instance = Expression.Parameter(typeof(TInstance), "instance");
			ParameterExpression body = Expression.Parameter(typeof(TBody), "body");
			MethodCallExpression call = Expression.Call(instance, methodInfo, body);

			return Expression.Lambda<Action<TInstance, TBody>>(call, instance, body).Compile();
		}
	}
}