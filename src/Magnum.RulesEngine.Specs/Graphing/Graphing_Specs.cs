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
namespace Magnum.RulesEngine.Specs.Graphing
{
	using System;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using ExecutionModel;
	using Model;
	using NUnit.Framework;
	using SemanticModel;
	using Visualizers;

	[TestFixture]
	public class Graphing_an_existing_engine
	{
		private Engine _engine;

		[SetUp]
		public void Setup()
		{
			_engine = new Engine();
			_engine.Add(CreateOnlineOrderRule());
			_engine.Add(CustomerIsSpecified());
			_engine.Add(CreateActiveNotPreferredRule());
			_engine.Add(CreatePreferredRule());
		}

		private RuleDeclaration CreateOnlineOrderRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Source == "Online";

			ConditionDeclaration condition = Declaration.Condition(exp);
			ConsequenceDeclaration consequence = Declaration.Consequence(() => Trace.WriteLine("Online Order"));

			return Declaration.Rule(new[] { condition }, new[] { consequence });
		}

		private RuleDeclaration CustomerIsSpecified()
		{
			Expression<Func<Order, bool>> exp = o => o.Customer != null;

			ConditionDeclaration condition = Declaration.Condition(exp);
			ConsequenceDeclaration consequence = Declaration.Consequence(() => Trace.WriteLine("Customer Order"));

			return Declaration.Rule(new[] { condition }, new[] { consequence });
		}

		private RuleDeclaration CreateActiveNotPreferredRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Customer.Preferred == false;
			ConditionDeclaration condition = Declaration.Condition(exp);

			Expression<Func<Order, bool>> exp2 = o => o.Customer.Active;
			ConditionDeclaration condition2 = Declaration.Condition(exp2);

			ConsequenceDeclaration consequence = Declaration.Consequence(() => Trace.WriteLine("Active, Not Preferred"));

			return Declaration.Rule(new[] { condition, condition2 }, new[] { consequence });
		}


		private RuleDeclaration CreatePreferredRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Customer.Preferred;
			ConditionDeclaration condition = Declaration.Condition(exp);

			ConsequenceDeclaration consequence = Declaration.Consequence(() => Trace.WriteLine("Preferred Customer"));

			return Declaration.Rule(new[] { condition }, new[] { consequence });
		}

		[Test]
		public void Should_generate_a_nice_graph_of_the_network()
		{
			GraphNodeVisitor visitor = new GraphNodeVisitor();
			_engine.Visit(visitor);

			visitor.ComputeShortestPath();
			visitor.GetGraph();
		}
	}
}