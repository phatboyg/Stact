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


	public class MethodActivity<TInstance> :
		ActivityBase<TInstance>
		where TInstance : class
	{
		readonly Action<TInstance> _method;
		readonly MethodInfo _methodInfo;

		public MethodActivity(State<TInstance> state, Event eevent, Expression<Func<TInstance, Action>> methodExpression)
			: base(state, eevent)
		{
			_methodInfo = new FindMethodCallVisitor().Find(methodExpression);
			if (_methodInfo == null)
			{
				throw new StateMachineConfigurationException("The expression method info could not be found: "
				                                                    + methodExpression);
			}

			_method = CompileMethod(_methodInfo);
		}

		public string MethodName
		{
			get { return _methodInfo.Name; }
		}

		public override void Execute(TInstance instance)
		{
			_method(instance);
		}

		public override void Execute<T>(TInstance instance, T body)
		{
			_method(instance);
		}

		static Action<TInstance> CompileMethod(MethodInfo methodInfo)
		{
			ParameterExpression instance = Expression.Parameter(typeof(TInstance), "instance");
			MethodCallExpression call = Expression.Call(instance, methodInfo);

			return Expression.Lambda<Action<TInstance>>(call, instance).Compile();
		}
	}
}