namespace Magnum.RulesEngine.Specs
{
	using System;
	using System.Collections.Generic;

	public class RuleSet
	{
		private readonly List<IRuleDefinition> _ruleDefinitions = new List<IRuleDefinition>();

		protected RuleDefinition Rule(Action<RuleDefinition> action)
		{
			var def = new RuleDefinition();

			action(def);

			_ruleDefinitions.Add(def);

			return def;
		}
	}
}