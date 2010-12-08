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
	public class TransitionActivity<TInstance> :
		ActivityBase<TInstance>
		where TInstance : class
	{
		readonly StateAccessor<TInstance> _currentStateAccessor;
		readonly State<TInstance> _targetState;

		public TransitionActivity(StateAccessor<TInstance> currentStateAccessor, State<TInstance> state, Event eevent,
		                             State<TInstance> targetState)
			: base(state,eevent)
		{
			_currentStateAccessor = currentStateAccessor;
			_targetState = targetState;
		}

		public State TargetState
		{
			get { return _targetState; }
		}

		public override void Execute(TInstance instance)
		{
			State<TInstance> currentState = _currentStateAccessor.Get(instance);
			if (currentState == _targetState)
				return;

			if (currentState != null)
				currentState.RaiseEvent(instance, currentState.Exit);

			_currentStateAccessor.Set(instance, _targetState);

			_targetState.RaiseEvent(instance, _targetState.Entry);
		}

		public override void Execute<TBody>(TInstance instance, TBody body)
		{
			Execute(instance);
		}
	}
}