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
namespace Stact.StateMachine.Channels
{
	using Magnum.StateMachine;
	using Stact.Channels;


	public class StateMachineEventChannel<T, TChannel> :
		Channel<TChannel>
		where T : StateMachine<T>
	{
		readonly Event<TChannel> _event;
		readonly T _instance;

		public StateMachineEventChannel(T instance, Event<TChannel> @event)
		{
			_instance = instance;
			_event = @event;
		}

		public void Send(TChannel message)
		{
			_instance.RaiseEvent(_event, message);
		}
	}
}