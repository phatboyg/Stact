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
	using Stact.StateMachine;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Specifying_an_event_handler_that_can_happen_anytime
	{
		[Test]
		public void Should_bind_the_event_handler_to_every_state()
		{
			var machine = new AnytimeStateMachineTest();

			machine.RaiseEvent(AnytimeStateMachineTest.BigBang);

			machine.BigBangOccurred.ShouldBeTrue();
			machine.CurrentState.ShouldEqual(AnytimeStateMachineTest.Completed);
		}

		[Test]
		public void Should_bind_to_event_and_not_change_state()
		{
			var machine = new AnytimeStateMachineTest();

			machine.RaiseEvent(AnytimeStateMachineTest.Supernova);

			machine.SupernovaOccurred.ShouldBeTrue();
			machine.CurrentState.ShouldEqual(AnytimeStateMachineTest.Initial);
			machine.UniverseIsExpanding.ShouldBeFalse();
		}

		[Test]
		public void Should_bind_to_enter_event()
		{
			var machine = new AnytimeStateMachineTest();

			machine.RaiseEvent(AnytimeStateMachineTest.BigBang);

			machine.UniverseIsExpanding.ShouldBeTrue();
		}

		[Test]
		public void Should_be_visible_in_the_visualizer()
		{
			var machine = new AnytimeStateMachineTest();

			StateMachineInspector.Trace(machine);
		}


		public class AnytimeStateMachineTest :
			StateMachine<AnytimeStateMachineTest>
		{
			static AnytimeStateMachineTest()
			{
				Define(() =>
					{
						Anytime(
							When(BigBang)
								.Call(sm => SetBigBangOccurred(sm))
								.Complete(),
							When(Supernova)
								.Then(sm => sm.SupernovaOccurred = true),
							When(Completed.Enter)
								.Call(sm => sm.OnCompleted()));
					});
			}

			private static void SetBigBangOccurred(AnytimeStateMachineTest machine)
			{
				machine.BigBangOccurred = true;
			}

			private bool OnCompleted()
			{
				return UniverseIsExpanding = true;
			}

			public bool UniverseIsExpanding { get; private set; }

			public bool SupernovaOccurred { get; private set; }

			public bool BigBangOccurred { get; private set; }

			public static State Initial { get; set; }
			public static State Completed { get; set; }

			public static Event BigBang { get; set; }
			public static Event Supernova { get; set; }
		}
	}
}