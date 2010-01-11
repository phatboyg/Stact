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
namespace Magnum.RulesEngine.DSL
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using SemanticModel;

	public class Rule
	{
		public static IEnumerable<RuleDeclaration> Declare<TRule>(Action<RuleConfigurator<TRule>> configure)
			where TRule : class
		{
			var configurator = new DynamicRuleConfigurator<TRule>();
			configure(configurator);

			return configurator.Rules();
		}
	}

	public class DynamicRuleConfigurator<T> :
		RuleConfigurator<T>
		where T : class
	{
		private readonly List<ConditionDeclaration> _conditions = new List<ConditionDeclaration>();
		private readonly List<ConsequenceDeclaration> _consequences = new List<ConsequenceDeclaration>();
		private readonly List<RuleDeclaration> _rules = new List<RuleDeclaration>();

		public RuleConfigurator<T> When<TCondition>()
			where TCondition : Condition<T>, new()
		{
			CompletePendingRules();

			ConditionDeclaration condition = Declaration.Condition<T>(x => new TCondition().IsSatisfiedBy(x));
			_conditions.Add(condition);

			return this;
		}

		public RuleConfigurator<T> When(Expression<Func<T, bool>> expression)
		{
			CompletePendingRules();

			ConditionDeclaration condition = Declaration.Condition(expression);
			_conditions.Add(condition);

			return this;
		}

		public RuleConfigurator<T> Then<TConsequence>()
			where TConsequence : Consequence<T>, new()
		{
			ConsequenceDeclaration consequence = Declaration.Consequence<T>(x => new TConsequence().Execute(x));
			_consequences.Add(consequence);

			return this;
		}

		public RuleConfigurator<T> Then(Expression<Action<RuleContext<T>>> action)
		{
			ConsequenceDeclaration consequence = Declaration.Consequence(action);
			_consequences.Add(consequence);

			return this;
		}

		public IEnumerable<RuleDeclaration> Rules()
		{
			CompletePendingRules();

			return _rules;
		}

		private void CompletePendingRules()
		{
			if (_consequences.Count > 0)
			{
				if (_conditions.Count == 0)
					_conditions.Add(CreateDefaultCondition());

				_rules.Add(Declaration.Rule(_conditions.ToArray(), _consequences.ToArray()));

				_consequences.Clear();
			}

			_conditions.Clear();
		}

		private static ConditionDeclaration CreateDefaultCondition()
		{
			Expression<Func<T, bool>> exp = o => true;
			ConditionDeclaration condition = Declaration.Condition(exp);

			return condition;
		}
	}
}