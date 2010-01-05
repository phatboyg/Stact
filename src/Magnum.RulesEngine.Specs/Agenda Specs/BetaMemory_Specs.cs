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
namespace BetaMemory_Specs
{
	using System;
	using System.Diagnostics;
	using Magnum.RulesEngine;
	using Magnum.RulesEngine.ExecutionModel;
	using Magnum.RulesEngine.Specs.Model;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class BetaMemory_Specs
	{
		private Customer _customer;
		private ActionNode<Customer> _actionNode;
		private LeafNode<Customer> _leafNode;

		[SetUp]
		public void Setup()
		{
			_customer = new Customer { Preferred = true };

			_actionNode = new ActionNode<Customer>(x => Trace.WriteLine("Called for " + x.Element.Object.Preferred));

			_leafNode = new LeafNode<Customer>();
			_leafNode.AddSuccessor(_actionNode);

		}

		[Test]
		public void FirstTestName()
		{
			var junction = new MemoryJunction<Customer>(_leafNode.Activate);

			var successors = new Activatable<Customer>[] {junction};

			var memoryA = new BetaMemory<Customer>(successors);

			var context = MockRepository.GenerateMock<RuleContext<Customer>>();
			
			var element = MockRepository.GenerateMock<WorkingMemoryElement<Customer>>();
			element.Stub(x => x.Object).Return(_customer);
			context.Stub(x => x.Element).Return(element);

			context.Expect(x => x.EnqueueAgendaAction(0, null)).IgnoreArguments()
				.WhenCalled(invocation =>
					{
						var action = invocation.Arguments[1] as Action;
						if (action != null) action();
					});

			memoryA.Activate(context);

			memoryA.ActivateSuccessors();

			context.VerifyAllExpectations();
		}

		[Test]
		public void Pulling_an_element_through_two_memories_should_merge_properly()
		{
			var junction = new MemoryJunction<Customer>(_leafNode.Activate);

			var successors = new Activatable<Customer>[] {junction};

			var memoryB = new BetaMemory<Customer>(successors);

			var joinJunction = new MemoryJunction<Customer>(memoryB.RightActivate);

			var succB = new Activatable<Customer>[] {joinJunction};

			var memoryA = new BetaMemory<Customer>(succB);

			var context = MockRepository.GenerateMock<RuleContext<Customer>>();
			
			var element = MockRepository.GenerateMock<WorkingMemoryElement<Customer>>();
			element.Stub(x => x.Object).Return(_customer);
			context.Stub(x => x.Element).Return(element);

			context.Expect(x => x.EnqueueAgendaAction(0, null)).IgnoreArguments()
				.WhenCalled(invocation =>
					{
						var action = invocation.Arguments[1] as Action;
						if (action != null) action();
					});

			memoryA.Activate(context);
			memoryB.Activate(context);

			memoryA.ActivateSuccessors();
			memoryB.ActivateSuccessors();

			context.VerifyAllExpectations();
		}
	}
}