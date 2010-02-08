namespace Magnum.RulesEngine.Specs.Graphing
{
	using System.IO;
	using System.Reflection;
	using Magnum.Specs.StateMachine;
	using NUnit.Framework;
	using StateMachine;
	using Visualizers;

	[TestFixture]
	public class StateMachineGraph_Specs
	{
		[Test]
		public void I_want_to_see_you_pretty()
		{
			var machine = new OrderStateMachine();

			var generator = new StateMachineGraphGenerator();

			string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "graph.png");

			generator.SaveGraphToFile(machine, 2560, 1920, filename);
		}
	}
}
