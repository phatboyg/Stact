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
	using Magnum.StateMachine;
	using NUnit.Framework;

	[TestFixture]
	public class Combining_a_list_of_events
	{
		[Test]
		public void Should_result_in_a_combined_event()
		{
			var machine = new CombinedStateMachineTest();

			machine.AllState.ShouldEqual(0);

			machine.RaiseEvent(CombinedStateMachineTest.First);
			machine.CurrentState.ShouldEqual(CombinedStateMachineTest.Initial);

			machine.RaiseEvent(CombinedStateMachineTest.Second);
			machine.CurrentState.ShouldEqual(CombinedStateMachineTest.Completed);
		}


		public class CombinedStateMachineTest :
			StateMachine<CombinedStateMachineTest>
		{
			static CombinedStateMachineTest()
			{
				Define(() =>
					{
						Combine(First, Second)
							.Into(All, x => x.AllState);

						Anytime(
							When(All)
								.TransitionTo(Completed));
					});
			}

			public virtual int AllState { get; private set; }

			public static State Initial { get; set; }
			public static State Completed { get; set; }


			public static Event First { get; set; }
			public static Event Second { get; set; }

			public static Event All { get; set; }
		}
	}
}