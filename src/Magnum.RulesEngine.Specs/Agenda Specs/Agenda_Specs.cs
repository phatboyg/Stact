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
namespace Agenda_Specs
{
	using System;
	using Magnum.RulesEngine;
	using Magnum.RulesEngine.ExecutionModel;
	using Magnum.RulesEngine.Specs.Model;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class Agenda_Specs
	{
		[Test]
		public void Building_a_agenda_from_the_consequence_rocks()
		{
			var context = MockRepository.GenerateMock<RuleContext<Customer>>();
			context.Expect(x => x.EnqueueAgendaAction(0, null)).IgnoreArguments();

			Action<int> action = null;
			action = MockRepository.GenerateMock<Action<int>>();
			var actionNode = new ActionNode(() => action(27));

			actionNode.Activate(context);

			context.VerifyAllExpectations();
		}

		[Test]
		public void Building_an_agenda_from_the_action_node_of_t()
		{
			var customer = new Customer();

			var wme = MockRepository.GenerateMock<WorkingMemoryElement<Customer>>();
			wme.Stub(x => x.Object).Return(customer);

			var context = MockRepository.GenerateMock<RuleContext<Customer>>();
			context.Stub(x => x.Element).Return(wme);

			context.Expect(x => x.EnqueueAgendaAction(0, null)).IgnoreArguments();

			var action = MockRepository.GenerateMock<Action<RuleContext<Customer>>>();

			var node = new ActionNode<Customer>(x => action(x));

			node.Activate(context);


			context.VerifyAllExpectations();
		}

		[Test]
		public void The_agenda_should_be_invoked_in_priority_order_lowest_to_highest()
		{
			Action<int> action = null;
			action = MockRepository.GenerateMock<Action<int>>();
			using (action.GetMockRepository().Ordered())
			{
				action.Expect(x => x(42));
				action.Expect(x => x(27));

				Agenda a = new PriorityQueueAgenda();

				a.Add(1000, () => action(27));
				a.Add(() => action(42));

				a.Execute();

				action.VerifyAllExpectations();
			}
		}
	}
}