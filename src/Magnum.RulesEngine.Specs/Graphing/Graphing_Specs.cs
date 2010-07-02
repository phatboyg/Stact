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
	using System.IO;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.Visualizers.RulesEngine;
	using Model;
	using NUnit.Framework;
	using SemanticModel;


	[TestFixture]
	public class Graphing_an_existing_engine
	{
		[SetUp]
		public void Setup()
		{
			//TODO Dru needs to see this

			_engine = new MagnumRulesEngine();
			_engine.Add(CreateOnlineOrderRule());
			_engine.Add(CustomerIsSpecified());
			_engine.Add(CreateActiveNotPreferredRule());
			_engine.Add(CreatePreferredRule());
			_engine.Add(CreateOnlinePreferredOrderRule());
		}

		private MagnumRulesEngine _engine;

		private RuleDeclaration CreateOnlineOrderRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Source == "Online";

			ConditionDeclaration condition = Declaration.Condition(exp);
			ConsequenceDeclaration consequence = Declaration.Consequence<Order>(x => x.IsOnline());

			return Declaration.Rule(new[] {condition}, new[] {consequence});
		}

		private RuleDeclaration CreateOnlinePreferredOrderRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Source == "Online";
			ConditionDeclaration condition = Declaration.Condition(exp);

			Expression<Func<Order, bool>> exp2 = o => o.Customer.Preferred;
			ConditionDeclaration condition2 = Declaration.Condition(exp2);

			ConsequenceDeclaration consequence = Declaration.Consequence<Order>(x => x.IsOnlinePreferred());

			return Declaration.Rule(new[] {condition, condition2}, new[] {consequence});
		}

		private RuleDeclaration CustomerIsSpecified()
		{
			Expression<Func<Order, bool>> exp = o => o.Customer != null;

			ConditionDeclaration condition = Declaration.Condition(exp);
			ConsequenceDeclaration consequence = Declaration.Consequence<Order>(x => x.HasCustomer());

			return Declaration.Rule(new[] {condition}, new[] {consequence});
		}

		private RuleDeclaration CreateActiveNotPreferredRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Customer.Preferred == false;
			ConditionDeclaration condition = Declaration.Condition(exp);

			Expression<Func<Order, bool>> exp2 = o => o.Customer.Active;
			ConditionDeclaration condition2 = Declaration.Condition(exp2);

			ConsequenceDeclaration consequence = Declaration.Consequence<Order>(x => x.IsActiveNotPreferred());

			return Declaration.Rule(new[] {condition, condition2}, new[] {consequence});
		}

		private RuleDeclaration CreatePreferredRule()
		{
			Expression<Func<Order, bool>> exp = o => o.Customer.Preferred;
			ConditionDeclaration condition = Declaration.Condition(exp);

			ConsequenceDeclaration consequence = Declaration.Consequence<Order>(x => x.IsPreferred());

			return Declaration.Rule(new[] {condition}, new[] {consequence});
		}

		[Test]
		public void Should_generate_a_nice_graph_of_the_network()
		{
			string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "graph.png");

			var generator = new RulesEngineGraphGenerator();

			generator.SaveGraphToFile(_engine.GetGraphData(), 2560, 1920, filename);
		}
	}

	public static class xxx
	{
		public static void HasCustomer(this RuleContext<Order> x)
		{
		}

		public static void IsPreferred(this RuleContext<Order> x)
		{
		}

		public static void IsActiveNotPreferred(this RuleContext<Order> x)
		{
		}

		public static void IsOnlinePreferred(this RuleContext<Order> x)
		{
		}

		public static void IsOnline(this RuleContext<Order> x)
		{
		}
	}
}