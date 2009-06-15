// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.StateMachine
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Collections;

	public class EventActionList<T> :
		IEnumerable<EventAction<T>>
		where T : StateMachine<T>
	{
		private readonly List<EventAction<T>> _actions = new List<EventAction<T>>();

		public IEnumerator<EventAction<T>> GetEnumerator()
		{
			return _actions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(Action<T> action)
		{
			_actions.Add(new LambdaAction<T>((x, e) => action(x)));
		}

		public void Add(Action<T, BasicEvent<T>> action)
		{
			_actions.Add(new LambdaAction<T>(action));
		}

		public void Add<TData>(Action<T, TData> action)
			where TData : class
		{
			_actions.Add(new LambdaAction<T, TData>((x, e, d) => action(x, d)));
		}

		public void Add<TData>(Action<T, DataEvent<T, TData>, TData> action)
			where TData : class
		{
			_actions.Add(new LambdaAction<T, TData>(action));
		}

		public void Add(Expression<Action<T>> expression)
		{
			_actions.Add(new ExpressionAction<T>(expression));
		}

		public void Add<TData>(Expression<Action<T, TData>> expression) 
			where TData : class
		{
			_actions.Add(new ExpressionAction<T,TData>(expression));
		}

		public void AddStateTransition(State state)
		{
			_actions.Add(new TransitionToAction<T>(state));
		}

		public void Execute(T stateMachine, Event @event, object parameter)
		{
			_actions.Each(action => action.Execute(stateMachine, @event, parameter));
		}
	}
}