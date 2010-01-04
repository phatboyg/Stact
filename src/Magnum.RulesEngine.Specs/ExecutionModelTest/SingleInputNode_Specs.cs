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
namespace Magnum.RulesEngine.Specs.ExecutionModelTest
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using DateTimeExtensions;
	using ExecutionModel;
	using Model;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestFramework;

	[TestFixture]
	public class Loading_up_a_single_node
	{
		[Test]
		public void Should_property_make_things_work()
		{
			ActionConsequenceNode node = new ActionConsequenceNode(x => Trace.WriteLine("Hello"));

			var customer = new Customer {Preferred = true};

			var context = MockRepository.GenerateMock<RuleContext>();
			context.Stub(x => x.Match<Customer>()).Return(new[] {customer});

			ConditionNode<Customer> isPreferred = new ConditionNode<Customer>(x => x.Preferred);
			ConditionNode<Customer> isActive = new ConditionNode<Customer>(x => x.Active);


			//conditionNode.

			//node.Activate(context);


			SingleInputTreeNode singleInputTreeNode = new SingleInputTreeNode();

			singleInputTreeNode.Add(isPreferred);
			singleInputTreeNode.Add(isActive);

			var element = new SessionWorkingMemoryElement<Customer>(null, customer);

			var ruleContext = new SessionRuleContext<Customer>(null, element);

			singleInputTreeNode.Activate(ruleContext);
		}











		[Test]
		public void Sometimes_a_prototype_is_just_that()
		{


			Customer customer = new Customer() { Preferred = true, Active = true, LastActivity = DateTime.Now - 5.Days() };

			ANode<Customer> node = new ANode<Customer>(x => x.Preferred);

			AMemory<Customer> memory = node.CreateAlphaMemory(customer);

			memory.IsSatisfied().ShouldBeTrue();
			memory.IsSatisfied().ShouldBeTrue();

		}
	}

	public class ANode<T>
	{
		private readonly Expression<Func<T, bool>> _expression;
		private Func<T, bool> _eval;

		public ANode(Expression<Func<T, bool>> expression)
		{
			_expression = expression;
			_eval = _expression.Compile();
		}

		public AMemory<T> CreateAlphaMemory(T item)
		{
			Func<bool> eval;
			eval = () =>
			{
				bool result = _eval(item);
				Trace.WriteLine("Eval ran");

				eval = () => result;

				return result;
			};

			Func<bool> satisfied = () =>
				{

					return eval();
				};

			return new AMemory<T>(satisfied);
		}
	}

	public class AMemory<T>
	{
		private readonly Func<bool> _eval;

		public AMemory(Func<bool> eval)
		{
			_eval = eval;
		}

		public bool IsSatisfied()
		{
			return _eval();
		}
	}


	/*
	 * 
	 * Working Memory is an arbitrary space
	 * 
	 * An single node match produces a single node memory context (RuleContext<T>)
	 * 
	 * alpha node, alpha memory
	 * 
	 * alpha memory invoke nodes in the beta tree
	 * 
	 * produce a beta memory with multiple objects asserted
	 * 
	 * when the leaf is reached, a production is queued to an agenda
	 * a production is built as a consequence with a priority?
	 * 
	 * 
	 * 
	 * 
	 */
}