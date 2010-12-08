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
	public abstract class ActivityBase<TInstance> :
		Activity<TInstance>
		where TInstance : class
	{
		readonly Event _event;
		readonly State<TInstance> _state;

		protected ActivityBase(State<TInstance> state, Event @event)
		{
			_state = state;
			_event = @event;
		}

		public State State
		{
			get { return _state; }
		}

		public Event Event
		{
			get { return _event; }
		}

		public abstract void Execute(TInstance instance);
		public abstract void Execute<TBody>(TInstance instance, TBody body);

		public virtual void Accept(StateMachineVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}