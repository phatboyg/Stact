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
	using System.Diagnostics;
	using System.Linq.Expressions;
	using Events;
	using ExecutionModel;
	using NUnit.Framework;
	using Rhino.Mocks;
	using SemanticModel;

	[TestFixture]
	public class ProcessingAnEvent_Specs
	{
		private OrderSubmitted _order;
		private Engine _engine;
		private Action<OrderSubmitted> _overLimit;
		private Action<OrderSubmitted> _underLimit;

		[SetUp]
		public void Setup()
		{
			_order = new OrderSubmitted();

			_overLimit = MockRepository.GenerateMock<Action<OrderSubmitted>>();
			_overLimit.Expect(x => x(_order));

			_underLimit = MockRepository.GenerateMock<Action<OrderSubmitted>>();
			_underLimit.Expect(x => x(_order)).Repeat.Never();

			_engine = new Engine();

			AddOverLimitRule();
			AddUnderLimitRule();

			StringNodeVisitor visitor = new StringNodeVisitor();
			_engine.Visit(visitor);

			Trace.WriteLine(visitor.Result);
		}

		private void AddOverLimitRule()
		{
			Expression<Func<OrderSubmitted, bool>> exp = o => o.Amount > 1000.00m;

			ConditionDeclaration condition = Declaration.Condition(exp);
			ConsequenceDeclaration<OrderSubmitted> consequence = Declaration.Consequence<OrderSubmitted>(o => _overLimit(o.Element.Object));

			RuleDeclaration rule = Declaration.Rule(new[] {condition}, new[] {consequence});

			_engine.Add(rule);
		}

		private void AddUnderLimitRule()
		{
			Expression<Func<OrderSubmitted, bool>> exp = o => o.Amount < 50.00m;

			ConditionDeclaration condition = Declaration.Condition(exp);
			ConsequenceDeclaration<OrderSubmitted> consequence = Declaration.Consequence<OrderSubmitted>(o => _underLimit(o.Element.Object));

			RuleDeclaration rule = Declaration.Rule(new[] {condition}, new[] {consequence});

			_engine.Add(rule);
		}

		[Test]
		public void FirstTestName()
		{
			_order.Amount = 10000.00m;

			using (var session = _engine.CreateSession())
			{
				session.Assert(_order);
				session.Run();
			}

			_overLimit.VerifyAllExpectations();
			_underLimit.VerifyAllExpectations();
		}
	}
}