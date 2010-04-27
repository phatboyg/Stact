namespace Magnum.Specs.Channels
{
	using Fibers;
	using Magnum.Channels;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Subscribing_to_an_untyped_channel_adapter
	{
		[Test]
		public void Should_register_my_consumer()
		{
			var input = new UntypedChannelAdapter(new SynchronousFiber());

			var futureA = new Future<TestMessage>();
			var consumerA = new ConsumerChannel<TestMessage>(new SynchronousFiber(), futureA.Complete);

			var futureB = new Future<TestMessage>();
			var consumerB = new ConsumerChannel<TestMessage>(new SynchronousFiber(), futureB.Complete);

			using (var subscription = input.Subscribe())
			{
				subscription.Add(consumerA);
				new TraceChannelVisitor().Visit(input);

				subscription.Add(consumerB);
				new TraceChannelVisitor().Visit(input);

				input.Send(new TestMessage());
			}

			futureA.IsCompleted.ShouldBeTrue();
			futureB.IsCompleted.ShouldBeTrue();
		}

		private class TestMessage
		{
		}
	}
}
