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
		[SetUp]
		public void Setup()
		{
			_customer = new Customer {Preferred = true};

			_actionNode = new ActionNode<Customer>(x => Trace.WriteLine("Called for " + x.Element.Object.Preferred));

			_leafNode = new LeafNode<Customer>();

			_agenda = new PriorityQueueAgenda();

			_context = MockRepository.GenerateMock<RuleContext<Customer>>();
			var element = MockRepository.GenerateMock<WorkingMemoryElement<Customer>>();
			element.Stub(x => x.Object).Return(_customer);
			_context.Stub(x => x.Element).Return(element);

			_context.Expect(x => x.EnqueueAgendaAction(0, null))
				.IgnoreArguments()
				.Repeat.AtLeastOnce()
				.WhenCalled(invocation =>
					{
						var priority = (int) invocation.Arguments[0];
						var action = invocation.Arguments[1] as Action;

						_agenda.Add(priority, action);
					});
		}

		private Customer _customer;
		private ActionNode<Customer> _actionNode;
		private LeafNode<Customer> _leafNode;
		private RuleContext<Customer> _context;
		private Agenda _agenda;

		[Test]
		public void FirstTestName()
		{
			var junction = new MemoryJunction<Customer>(_leafNode.Activate);
			junction.AddSuccessor(_actionNode);

			var memoryA = new BetaMemory<Customer>(junction);

			memoryA.Activate(_context);

			_agenda.Execute();

			_context.VerifyAllExpectations();
		}

		[Test]
		public void One_more_level_of_indirection()
		{
			var junction = new MemoryJunction<Customer>(_leafNode.Activate);
			junction.AddSuccessor(_actionNode);

			var alphaNode = new AlphaNode<Customer>();
			alphaNode.AddSuccessor(junction);

			alphaNode.Activate(_context);

			//var memoryA = new BetaMemory<Customer>(junction);



			//memoryA.Activate(_context);

			_agenda.Execute();

			_context.VerifyAllExpectations();
		}

		[Test]
		public void Pulling_an_element_through_two_memories_should_merge_properly()
		{
			var junction = new MemoryJunction<Customer>(_leafNode.Activate);
			junction.AddSuccessor(_actionNode);

			var memoryB = new BetaMemory<Customer>(junction);

			var joinJunction = new MemoryJunction<Customer>(memoryB.RightActivate);

			var memoryA = new BetaMemory<Customer>(joinJunction);

			memoryA.Activate(_context);
			memoryB.Activate(_context);

			_agenda.Execute();

			_context.VerifyAllExpectations();
		}
	}

	[TestFixture]
	public class Context_and_throughout_usage
	{
		[SetUp]
		public void Setup()
		{
			_customer = new Customer {Preferred = true};

			_actionNode = new ActionNode<Customer>(x => Trace.WriteLine("Called for " + x.Element.Object.Preferred));

			_leafNode = new LeafNode<Customer>();

			var element = MockRepository.GenerateMock<WorkingMemoryElement<Customer>>();
			element.Stub(x => x.Object).Return(_customer);

			var session = MockRepository.GenerateMock<StatefulSession>();;

			_context = new SessionRuleContext<Customer>(session, element);
		}

		private Customer _customer;
		private ActionNode<Customer> _actionNode;
		private LeafNode<Customer> _leafNode;
		private RuleContext<Customer> _context;

		[Test]
		public void FirstTestName()
		{
			var junction = new MemoryJunction<Customer>(_leafNode.Activate);
			junction.AddSuccessor(_actionNode);

			var memoryA = new BetaMemory<Customer>(junction);

			memoryA.Activate(_context);

			_context.RunAgenda();
		}

		[Test]
		public void One_more_level_of_indirection()
		{
			var junction = new MemoryJunction<Customer>(_leafNode.Activate);
			junction.AddSuccessor(_actionNode);

			var alphaNode = new AlphaNode<Customer>();
			alphaNode.AddSuccessor(junction);

			alphaNode.Activate(_context);

			_context.RunAgenda();
		}

		[Test]
		public void Pulling_an_element_through_two_memories_should_merge_properly()
		{
			var junction = new MemoryJunction<Customer>(_leafNode.Activate);
			junction.AddSuccessor(_actionNode);

			var alphaNodeA = new AlphaNode<Customer>();
			alphaNodeA.AddSuccessor(junction);

			var joinJunction = new MemoryJunction<Customer>(alphaNodeA.RightActivate);

			var alphaNodeB = new AlphaNode<Customer>();
			alphaNodeB.AddSuccessor(joinJunction);

			alphaNodeA.Activate(_context);
			alphaNodeB.Activate(_context);

			_context.RunAgenda();
		}
	}
}