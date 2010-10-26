namespace Stact.Specs
{
	using System.Diagnostics;
	using Internal;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using Routing;
	using Routing.Visualizers;


	[Scenario]
	public class Show_me_visualization
	{
		[Then]
		public void Should_display_the_empty_network()
		{
			var received = new Future<A>();

			var engine = new DynamicRoutingEngine(new SynchronousFiber());

			engine.Send(new A());

			Trace.WriteLine("Before Receive");
			var visualizer = new RoutingEngineTextVisualizer();
			visualizer.Visit(engine);

			engine.Receive<A>(received.Complete);

			Trace.WriteLine("After Receive");
			visualizer.Visit(engine);

			received.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}


		class A
		{
		}
	}
}