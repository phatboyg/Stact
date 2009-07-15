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
namespace Magnum.Specs.StateMachine
{
	using System;
	using Magnum.StateMachine;
	using NUnit.Framework;

	[TestFixture]
	public class EventException_Specs
	{
		[Test]
		public void An_exception_during_an_event_handler_should_be_handled()
		{
			var machine = new ExceptionalStateMachine();

			machine.RaiseEvent(ExceptionalStateMachine.BadNews);

			machine.CurrentState.ShouldEqual(ExceptionalStateMachine.Failed);
		}
	}


	public class ExceptionalStateMachine :
		StateMachine<ExceptionalStateMachine>
	{
		static ExceptionalStateMachine()
		{
			Define(() =>
				{
					Initially(
						When(BadNews)
							.Then(s =>
								{
									// at this point, we need to fail miserably
									throw new InvalidOperationException("TIME TO DIE!");
								},
							      InCaseOf<InvalidOperationException>().TransitionTo(Failed),
							      InCaseOf<ArgumentNullException>().TransitionTo(Initial))
							.TransitionTo(Completed));
				});
		}

		public static State Initial { get; set; }
		public static State Failed { get; set; }
		public static State Completed { get; set; }

		public static Event BadNews { get; set; }
	}
}