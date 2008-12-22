namespace Magnum.RulesEngine.Specs
{
	using System;
	using System.Linq.Expressions;

	public class RuleDefinition : IRuleDefinition
	{
		public RuleDefinition When<T>(Expression<Func<T, bool>> expression)
		{
			return this;
		}

		public RuleDefinition When<T>(Expression<Func<T, RuleViolation>> expression)
		{
			return this;
		}
	}
}