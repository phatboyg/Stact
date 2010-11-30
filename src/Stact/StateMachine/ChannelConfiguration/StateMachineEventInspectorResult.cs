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
namespace Stact.StateMachine.ChannelConfiguration
{
	using System;
	using System.Collections.Generic;
	using Configuration.Internal;
	using Internal;
	using Magnum.StateMachine;


	public interface StateMachineEventInspectorResult<T>
		where T : StateMachine<T>
	{
		Event GenericEvent { get; }
		Type EventType { get; }

		void Connect(ChannelConfiguratorConnection configurator, Fiber fiber, T instance);

		InstanceChannelPolicy<T, TChannel> GetPolicy<TChannel>(InstanceProvider<T, TChannel> missingInstanceProvider);
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

		public Event GenericEvent
		{
			get { return Event; }
		}

		public Type EventType
		{
			get { return typeof(V); }
		}

		public void Connect(ChannelConfiguratorConnection configurator, Fiber fiber, T instance)
		{
			// TODO kill this crap
			configurator.AddChannel(fiber, x => new ConsumerChannel<V>(x, m => instance.RaiseEvent(Event, m)));
		}

		public InstanceChannelPolicy<T, TChannel> GetPolicy<TChannel>(InstanceProvider<T, TChannel> missingInstanceProvider)
		{
			bool includesInitial = false;
			bool includesOther = false;

			foreach (State state in AcceptingStates)
			{
				if (IsAny(state))
					includesInitial = includesOther = true;
				if (IsInitial(state))
					includesInitial = true;
				else
					includesOther = true;
			}

			if (includesInitial && includesOther)
				return new CreateOrUseExistingInstanceChannelPolicy<T, TChannel>(missingInstanceProvider);

			if (includesInitial)
				return new CreateInstanceChannelPolicy<T, TChannel>(missingInstanceProvider);

			return new ExistingInstanceChannelPolicy<T, TChannel>();
		}

		static bool IsAny(State state)
		{
			return state.Name == "Any";
		}

		static bool IsInitial(State state)
		{
			return state.Name == "Initial";
		}
	}
}