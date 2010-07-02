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
namespace Magnum.RulesEngine.Specs.FluentRuleSyntax
{
	using System.Diagnostics;
	using Conditions;
	using Consequences;
	using DSL;
	using Events;
	using NUnit.Framework;


	[TestFixture]
	public class When_declaring_a_rule_using_the_domain_specific_language
	{
		[SetUp]
		public void Setup()
		{
			_engine = new MagnumRulesEngine();
		}

		[TearDown]
		public void Teardown()
		{
		}

		private RulesEngine _engine;

		[Test]
		public void Should_be_able_to_add_pure_expressions_as_well_for_ease()
		{
			RuleSet ruleSet = Rule.Declare<OrderSubmitted>(rule =>
				{
					rule.Then(x => Trace.WriteLine("Order Amount: " + x.Object.Amount));

					rule.When(x => x.Amount > 100.00m)
						.Then(x => Trace.WriteLine("Order is over the limit"))
						.Then(x => x.RequestApproval());

					rule.When(x => x.Amount > 1000m)
						.Then(x => x.RequestClearance());
				});

//			RuleSet ruleSet2 = Rule.Declare<OrderSubmitted>(rule =>
//				{
//					rule.When(x => x.Amount > 100.00m)
//						.Then(x => Trace.WriteLine("BING"));
//				});


			_engine.Add(ruleSet);
//			_engine.Add(ruleSet2);

			using (StatefulSession session = _engine.CreateSession())
			{
				session.Assert(new OrderSubmitted {Amount = 123.45m});

				session.Run();
			}
		}

		[Test]
		public void Should_be_able_to_add_them_into_a_set_of_rules()
		{
			RuleSet ruleSet = Rule.Declare<OrderSubmitted>(rule =>
				{
					rule.Then<LogOrderDetails>();

					rule.When<OrderAmountIsOverLimit>()
						.Then<RequestOrderApproval>();
				});

			_engine.Add(ruleSet);

			using (StatefulSession session = _engine.CreateSession())
			{
				session.Assert(new OrderSubmitted {Amount = 123.45m});

				session.Run();
			}
		}
	}

	public static class Extensions
	{
		public static void RequestApproval(this RuleContext<OrderSubmitted> order)
		{
		}

		public static void RequestClearance(this RuleContext<OrderSubmitted> order)
		{
		}
	}
}