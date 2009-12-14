namespace Magnum.RulesEngine.Specs.ExecutionModelTest
{
	using Events;
	using NUnit.Framework;

	[TestFixture]
	public class ProcessingAnEvent_Specs
	{
		[Test]
		public void FirstTestName()
		{
			var obj = new OrderSubmitted();

			WorkingMemory memory = new HashSetWorkingMemory();
			memory.Add(obj);

			RuleSet ruleSet = null;

			ruleSet.Evaluate(memory);


			
		}
	}
}