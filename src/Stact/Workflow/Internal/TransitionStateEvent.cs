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
	public class TransitionStateEvent<TInstance> :
		StateEvent<TInstance>
		where TInstance : class
	{
		readonly StateAccessor<TInstance> _currentStateAccessor;
		readonly Event _event;
		readonly State<TInstance> _state;
		readonly State<TInstance> _targetState;

		public TransitionStateEvent(StateAccessor<TInstance> currentStateAccessor, State<TInstance> state, Event eevent,
		                            State<TInstance> targetState)
		{
			_currentStateAccessor = currentStateAccessor;
			_targetState = targetState;
			_event = eevent;
			_state = state;
		}

		public State TargetState
		{
			get { return _targetState; }
		}

		public State State
		{
			get { return _state; }
		}

		public Event Event
		{
			get { return _event; }
		}

		public void Accept(StateMachineVisitor visitor)
		{
			visitor.Visit(this);
		}

		public void Execute(TInstance instance)
		{
			ChangeCurrentState(instance);
		}

		public void Execute<TBody>(TInstance instance, TBody body)
		{
			ChangeCurrentState(instance);
		}

		void ChangeCurrentState(TInstance instance)
		{
			State<TInstance> currentState = _currentStateAccessor.Get(instance);

			if (currentState == _targetState)
				return;

			if (currentState != null)
				currentState.RaiseEvent(instance, currentState.Leave);

			_currentStateAccessor.Set(instance, _targetState);

			_targetState.RaiseEvent(instance, _targetState.Enter);
		}
	}
}