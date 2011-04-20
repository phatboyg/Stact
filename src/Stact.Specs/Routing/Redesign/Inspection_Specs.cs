namespace Stact.Specs.Redesign
{
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using MessageHeaders;
	using Routing;
	using Routing.Internal;
	using Routing.Visualizers;


	[Scenario]
	public class Inspecting_a_routing_engine
	{

		class A { }

		[Then]
		public void Should_navigate_properly()
		{
			RoutingEngine engine = new DynamicRoutingEngine(new PoolFiber());

			var foundA = new Future<AlphaNode<Message<A>>>();
			var foundJoin = new Future<JoinNode<Message<A>>>();

			engine.Configure(x =>
			{
				new MatchAlphaNode<Message<A>>(engine, alphaNode =>
				{
					foundA.Complete(alphaNode);

					new MatchJoinNode<Message<A>>(alphaNode, joinNode =>
					{
						foundJoin.Complete(joinNode);
					});

				});
			});

			foundA.WaitUntilCompleted(5.Seconds()).ShouldBeTrue("Message A alpha node not found");
			foundJoin.WaitUntilCompleted(5.Seconds()).ShouldBeTrue("Message A constant join node not found");

			// new RoutingEngineTextVisualizer().Visit(engine);
		}

		[Then]
		public void Should_match_a_join_node()
		{
			RoutingEngine engine = new DynamicRoutingEngine(new PoolFiber());

			var called = new Future<Message<A>>();

			engine.Configure(x =>
			{
				x.Add(new ConsumerNode<Message<A>>(new SynchronousFiber(), message => called.Complete(message)));
			});

			engine.Send<Message<A>>(new MessageImpl<A>(new A()));

			called.WaitUntilCompleted(5.Seconds()).ShouldBeTrue("Message was not delivered");

			// new RoutingEngineTextVisualizer().Visit(engine);
		}
	}
}