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
namespace Magnum.StateMachine.ChannelConfiguration
{
	using System.Collections.Generic;
	using Channels;
	using Channels.Configuration;
	using Fibers;


	public interface StateMachineEventInspectorResult<T>
		where T : StateMachine<T>
	{
		void Connect(ConnectionConfigurator configurator, Fiber fiber, T instance);
	}


	/// <summary>
	/// Describes an event, received by a state machine, and the in which that event is valid
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StateMachineEventInspectorResult<T, V> :
		StateMachineEventInspectorResult<T>
		where T : StateMachine<T>
	{
		public StateMachineEventInspectorResult(DataEvent<T, V> eevent, IEnumerable<State> states)
		{
			Event = eevent;
			AcceptingStates = new List<State>(states);
		}

		/// <summary>
		/// The event received by the state machine
		/// </summary>
		public DataEvent<T, V> Event { get; private set; }

		/// <summary>
		/// The states in which the event can be delivered to the state machine
		/// </summary>
		public IEnumerable<State> AcceptingStates { get; private set; }

		public void Connect(ConnectionConfigurator configurator, Fiber fiber, T instance)
		{
			var consumerChannel = new ConsumerChannel<V>(fiber, x => instance.RaiseEvent(Event, x));
			configurator.Add(consumerChannel);
		}
	}
}