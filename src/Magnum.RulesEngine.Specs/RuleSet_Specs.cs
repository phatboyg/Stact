namespace Magnum.RulesEngine.Specs
{
	using MbUnit.Framework;
	using Model;

	[TestFixture]
	public class RuleSet_Specs
	{
		[Test]
		public void A_fluent_rule_definition_format()
		{

		}



	}

	public class TestRuleSet : RuleSet
	{
		public TestRuleSet()
		{
			Rule(r =>
			{
				r.When<Person>(x => x.FirstName == "Chris");


			});
		}
	}
}
