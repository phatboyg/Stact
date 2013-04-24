namespace Stact.Specs
{
    using System.Diagnostics;
    using Headers;
    using Internal;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MessageHeaders;
    using Routing;
    using Routing.Visualizers;


    [Scenario]
    public class Show_me_visualization
    {
        [Then]
        public void Should_display_the_empty_network()
        {
            var received = new Future<A>();

            var engine = new MessageRoutingEngine();

            engine.Send(new MessageContext<A>(new A()));

            Trace.WriteLine("Before Receive");
//            var visualizer = new TraceRoutingEngineVisualizer();
//            visualizer.Show(engine);

            engine.Configure(x => x.Receive<A>(received.Complete));

            Trace.WriteLine("After Receive");
//            visualizer.Show(engine);

            received.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
        }


        [Then]
        public void Should_have_the_bits_without_the_message_first()
        {
            var engine = new MessageRoutingEngine();
//            var visualizer = new TraceRoutingEngineVisualizer();

            var received = new Future<A>();
            engine.Configure(x => x.Receive<A>(received.Complete));

            var block = new Future<int>();
            engine.Add(0, () =>
                {
  //                  visualizer.Show(engine);
                    block.Complete(0);
                });
            block.WaitUntilCompleted(2.Seconds());

            engine.Send(new MessageContext<SimpleImpl>(new SimpleImpl()));

            engine.Send(new MessageContext<A>(new A()));
            received.WaitUntilCompleted(2.Seconds());

            engine.Send(new MessageContext<B>(new B()));

            var receivedB = new Future<B>();
            engine.Configure(x => x.Receive<B>(receivedB.Complete));

            received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
            receivedB.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();

            //engine.Receive<A, B>(x => { });

    //        visualizer.Show(engine);
        }

        class A
        {
        }

        class B
        {
        }
    }
}