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
namespace Magnum.Common.StateMachine
{
	using System;
	using System.Linq.Expressions;

	public class StateEventAction<T>
		where T : StateMachine<T>
	{
		private State _resultState;

		public StateEventAction(Event raised)
		{
			RaisedEvent = raised;

			EventAction = (a, b, c) =>
				{
				};
		}

		public Event RaisedEvent { get; private set; }

		public Action<T, Event, object> EventAction { get; protected set; }

		public State ResultState
		{
			get { return _resultState; }
			protected set
			{
				if (_resultState != null)
					throw new InvalidOperationException("A transition state has already been defined");

				_resultState = value;
			}
		}

		public StateEventAction<T> Then(Action<T> action)
		{
			EventAction += (t, e, v) => action(t);

			return this;
		}

		public StateEventAction<T> Then(Action<T, BasicEvent<T>> action)
		{
			EventAction += (t, e, v) => action(t, e as BasicEvent<T>);

			return this;
		}

		public StateEventAction<T> TransitionTo(State state)
		{
			ResultState = state;

			return this;
		}

		public StateEventAction<T> Complete()
		{
			ResultState = StateMachine<T>.GetCompletedState();

			return this;
		}

		public virtual void Execute(T instance, Event eevent, object value)
		{
			EventAction(instance, eevent, value);

			if (ResultState != null)
				instance.ChangeCurrentState(ResultState);
		}
	}

	public class StateEventAction<T, V> :
		StateEventAction<T>
		where T : StateMachine<T>
		where V : class
	{
		private Func<V, bool> _checkCondition = (x) => true;

		public StateEventAction(Event raised)
			: base(raised)
		{
		}

		public Expression<Func<V, bool>> Condition { get; set; }

		public StateEventAction<T, V> And(Expression<Func<V, bool>> condition)
		{
			Condition = condition;

			_checkCondition = condition.Compile();

			return this;
		}

		public new StateEventAction<T, V> Then(Action<T> action)
		{
			EventAction += (t, e, v) => action(t);

			return this;
		}

		public StateEventAction<T, V> Then(Action<T, V> action)
		{
			EventAction += (t, e, v) => action(t, v as V);

			return this;
		}

		public StateEventAction<T, V> Then(Action<T, DataEvent<T, V>, V> action)
		{
			EventAction += (t, e, v) => action(t, e as DataEvent<T, V>, v as V);

			return this;
		}

		public new StateEventAction<T, V> TransitionTo(State state)
		{
			ResultState = state;

			return this;
		}

		public new StateEventAction<T, V> Complete()
		{
			ResultState = StateMachine<T>.GetCompletedState();

			return this;
		}

		public override void Execute(T instance, Event eevent, object value)
		{
			var data = value as V;
			if (data == null) return;

			if (!_checkCondition(data)) return;

			base.Execute(instance, eevent, value);
		}
	}
}