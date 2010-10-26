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

			var engine = new DynamicRoutingEngine(new PoolFiber());

			engine.Send(new A());

			Trace.WriteLine("Before Receive");
			var visualizer = new RoutingEngineTextVisualizer();
			visualizer.Visit(engine);

			engine.Receive<A>(received.Complete);

			Trace.WriteLine("After Receive");
			visualizer.Visit(engine);

			received.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}


		[Then]
		public void Should_have_the_bits_without_the_message_first()
		{
			var engine = new DynamicRoutingEngine(new PoolFiber());
			var visualizer = new RoutingEngineTextVisualizer();

			var received = new Future<A>();
			engine.Receive<A>(received.Complete);

			var block = new Future<int>();
			engine.Add(() =>
				{
					visualizer.Visit(engine);
					block.Complete(0);
				});
			block.WaitUntilCompleted(2.Seconds());

			engine.Send(new A());
			received.WaitUntilCompleted(2.Seconds());

			engine.Send(new B());

			var receivedB = new Future<B>();
			engine.Receive<B>(receivedB.Complete);

			received.WaitUntilCompleted(200.Seconds()).ShouldBeTrue();
			receivedB.WaitUntilCompleted(200.Seconds()).ShouldBeTrue();

			//engine.Receive<A, B>(x => { });

			visualizer.Visit(engine);
		}

		class A
		{
		}

		class B
		{
		}
	}
}