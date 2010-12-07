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


	public class SimpleStateEvent<TInstance> :
		StateEvent<TInstance>
		where TInstance : class
	{
		readonly Event _event;
		readonly State _state;

		public SimpleStateEvent(State state, Event e)
		{
			_state = state;
			_event = e;
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
			throw new NotImplementedException();
		}

		public void Execute<TBody>(TInstance instance, TBody body)
		{
			throw new NotImplementedException();
		}
	}
}