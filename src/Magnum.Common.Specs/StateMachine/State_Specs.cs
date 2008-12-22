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
namespace Magnum.Common.Specs.StateMachine
{
	using MbUnit.Framework;

	[TestFixture]
	public class State_Specs
	{
		[Test]
		public void States_should_automatically_be_created_for_the_class()
		{
			ExampleStateMachine stateMachine = new ExampleStateMachine();

			Assert.IsNotNull(ExampleStateMachine.Idle);

			Assert.AreEqual(ExampleStateMachine.Idle.Name, "Idle");
		}

		[Test]
		public void The_initial_state_should_be_set()
		{
			ExampleStateMachine stateMachine = new ExampleStateMachine();

			Assert.AreEqual(ExampleStateMachine.Idle, stateMachine.Current);
		}

		[Test]
		public void The_transitions_should_work()
		{
			ExampleStateMachine stateMachine = new ExampleStateMachine();

			stateMachine.Consume(new ExampleOrder());

			Assert.AreEqual(ExampleStateMachine.TakingOrder, stateMachine.Current);
		}

	}

	public class ExampleOrder
	{
	}
}