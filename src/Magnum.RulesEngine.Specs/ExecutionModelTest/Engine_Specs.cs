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
namespace Engine_Specs
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using Magnum.RulesEngine;
	using Magnum.RulesEngine.ExecutionModel;
	using Magnum.RulesEngine.SemanticModel;
	using Magnum.RulesEngine.Specs.Model;
	using Magnum.TestFramework;
	using NUnit.Framework;

	public class Given_a_complete_order
	{
		protected Order CurrentOrder { get; private set; }

		[Given]
		public void A_complete_order()
		{
			CurrentOrder = new Order
				{
					Customer = new Customer
						{
							Preferred = false,
							Active = true,
							LastActivity = SystemUtil.Now - 5.Days(),
						},
					Source = "Online",
					OrderLines = new List<OrderLine>
						{
							new OrderLine
								{
									ProductCode = 12345
								},
							new OrderLine
								{
									ProductCode = 67890
								},
						},
				};
		}
	}

	[TestFixture]
	public class Engine_Specs :
		Given_a_complete_order
	{
		private RuleDeclaration CreateOnlineOrderRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Source == "Online";

			ConditionDeclaration condition = Declaration.Condition(exp);
			ConsequenceDeclaration consequence = Declaration.Consequence(() => Trace.WriteLine("Online Order"));

			return Declaration.Rule(new[] {condition}, new[] {consequence});
		}

		private RuleDeclaration CreateCustomerExistsRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Customer != null;

			ConditionDeclaration condition = Declaration.Condition(exp);
			ConsequenceDeclaration consequence = Declaration.Consequence(() => Trace.WriteLine("Customer Exists"));

			return Declaration.Rule(new[] {condition}, new[] {consequence});
		}

		private RuleDeclaration CreateActiveNotPreferredRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Customer.Preferred == false;
			ConditionDeclaration condition = Declaration.Condition(exp);

			Expression<Func<Order, bool>> exp2 = o => o.Customer.Active;  
			ConditionDeclaration condition2 = Declaration.Condition(exp2);

			ConsequenceDeclaration consequence = Declaration.Consequence(() => Trace.WriteLine("Active, Not Preferred"));

			return Declaration.Rule(new[] {condition, condition2}, new[] {consequence});
		}


		private RuleDeclaration CreatePreferredRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Customer.Preferred;	
			ConditionDeclaration condition = Declaration.Condition(exp);

			ConsequenceDeclaration consequence = Declaration.Consequence(() => Trace.WriteLine("Preferred Customer"));

			return Declaration.Rule(new[] {condition}, new[] {consequence});
		}

		[Test, Explicit]
		public void FirstTestName()
		{
			var engine = new MagnumRulesEngine();

			RuleDeclaration rule = CreateOnlineOrderRule();
			engine.Add(rule);
			rule = CreateCustomerExistsRule();
			engine.Add(rule);
			rule = CreateActiveNotPreferredRule();
			engine.Add(rule);
			rule = CreatePreferredRule();
			engine.Add(rule);

			var visitor = new StringNodeVisitor();
			engine.Visit(visitor);

			Trace.WriteLine(visitor.Result);


			using (StatefulSession session = engine.CreateSession())
			{
				session.Assert(CurrentOrder);

				session.Run();
			}
		}
	}
}