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
	using Magnum.Reflection;


	public class MethodStateEvent<TInstance> :
		StateEvent<TInstance>
		where TInstance : class
	{
		readonly Event _event;
		readonly Action<TInstance> _eventAction;
		readonly State<TInstance> _state;
		readonly MethodInfo _methodInfo;

		public MethodStateEvent(State<TInstance> state, Event eevent,
		                        Expression<Func<TInstance, Action>> methodExpression)
		{
			_event = eevent;
			_state = state;
			_methodInfo = new MethodCallVisitor().Find(methodExpression);
			if (_methodInfo == null)
				throw new StateMachineWorkflowConfiguratorException("The expression method info could not be found: " + methodExpression);

			_eventAction = CompileEventAction(_methodInfo);
		}

		public State State
		{
			get { return _state; }
		}

		public Event Event
		{
			get { return _event; }
		}

		public string MethodName
		{
			get { return _methodInfo.Name; }
		}

		public void Accept(StateMachineVisitor visitor)
		{
			visitor.Visit(this);
		}

		public void Execute(TInstance instance)
		{
			_eventAction(instance);
		}

		public void Execute<T>(TInstance instance, T body)
		{
			_eventAction(instance);
		}

		static Action<TInstance> CompileEventAction(MethodInfo methodInfo)
		{
			var instance = Expression.Parameter(typeof(TInstance), "instance");

			var call = Expression.Call(instance, methodInfo);

			var lambda = Expression.Lambda<Action<TInstance>>(call, instance);

			return lambda.Compile();
		}
	}

	public class MethodStateEvent<TInstance, TBody> :
		StateEvent<TInstance>
		where TInstance : class
	{
		readonly Event _event;
		readonly Action<TInstance, TBody> _eventAction;
		readonly State<TInstance> _state;
		readonly MethodInfo _methodInfo;

		public MethodStateEvent(State<TInstance> state, Event eevent,
		                        Expression<Func<TInstance, Action<TBody>>> methodExpression)
		{
			_event = eevent;
			_state = state;
			_methodInfo = new MethodCallVisitor().Find(methodExpression);
			if (_methodInfo == null)
				throw new StateMachineWorkflowConfiguratorException("The expression method info could not be found: " + methodExpression);

			_eventAction = CompileEventAction(_methodInfo);
		}

		public State State
		{
			get { return _state; }
		}

		public Event Event
		{
			get { return _event; }
		}

		public string MethodName
		{
			get { return _methodInfo.Name; }
		}

		public void Accept(StateMachineVisitor visitor)
		{
			visitor.Visit(this);
		}

		public void Execute(TInstance instance)
		{
			throw new StateMachineWorkflowException("Expected body on message was not present: " + _event.Name);
		}

		public void Execute<T>(TInstance instance, T body)
		{
			if (typeof(TBody) != typeof(T))
				throw new StateMachineWorkflowException("Body type mismatch for message event: " + _event.Name);

			_eventAction(instance, (TBody)((object)body));
		}

		static Action<TInstance, TBody> CompileEventAction(MethodInfo methodInfo)
		{
			var instance = Expression.Parameter(typeof(TInstance), "instance");
			var body = Expression.Parameter(typeof(TBody), "body");

			var call = Expression.Call(instance, methodInfo, body);

			var lambda = Expression.Lambda<Action<TInstance, TBody>>(call, instance, body);

			return lambda.Compile();
		}
	}

	public class MethodCallVisitor :
		ExpressionVisitor
	{
		MethodInfo _methodInfo;

		public MethodInfo Find(Expression e)
		{
			Visit(e);

			return _methodInfo;
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			if (c.Type == typeof(MethodInfo))
				_methodInfo = c.Value as MethodInfo;

			return base.VisitConstant(c);
		}
	}
}