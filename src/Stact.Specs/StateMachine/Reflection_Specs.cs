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
namespace Stact.Specs.StateMachine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Stact.Extensions;
	using Stact.StateMachine;
	using Stact.TestFramework;


	[Scenario]
	public class When_a_state_machine_is_inspected_via_reflection
	{
		StateMachineMetadata _details;

		[When]
		public void A_state_machine_is_inspected_via_reflection()
		{
			Type type = typeof(ExampleStateMachine);

			var inspector = new StateMachineTypeInspector();

			_details = inspector.GetStateMachineDetails(type);
		}

		[Then]
		public void Should_return_the_metadata()
		{
			_details.ShouldNotBeNull();
		}

		[Then]
		public void Should_get_all_the_states()
		{
			_details.States.Count().ShouldEqual(5);
		}

		[Then]
		public void Should_get_all_the_events()
		{
			_details.Events.Count().ShouldEqual(7);
		}
	}


	public class StateMachineTypeInspector
	{
		public StateMachineMetadata GetStateMachineDetails(Type type)
		{
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);

			IEnumerable<State> states = GetStates(properties);
			IEnumerable<Event> events = GetEvents(properties);

			return new StateMachineMetadata(states, events);
		}

		IEnumerable<Event> GetEvents(IEnumerable<PropertyInfo> properties)
		{
			return properties
				.Where(x => x.PropertyType.Implements<Event>())
				.Select(x => x.GetValue(null, null))
				.Cast<Event>();
		}

		IEnumerable<State> GetStates(IEnumerable<PropertyInfo> properties)
		{
			return properties
				.Where(x => x.PropertyType.Implements<State>())
				.Select(x => x.GetValue(null, null))
				.Cast<State>();
		}
	}


	public class StateMachineMetadata
	{
		readonly IEnumerable<State> _states;
		readonly IEnumerable<Event> _events;

		public IEnumerable<Event> Events
		{
			get { return _events; }
		}

		public StateMachineMetadata(IEnumerable<State> states, IEnumerable<Event> events)
		{
			_states = states;
			_events = events;
		}

		public IEnumerable<State> States
		{
			get { return _states; }
		}

	}
}