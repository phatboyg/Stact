namespace Magnum.Specs.Channels
{
	using System.Collections.Generic;
	using Magnum.Actions;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;

	[TestFixture]
	public class Visiting_a_channel_network
	{
		[Test]
		public void Should_capture_all_of_the_nodes_involved()
		{
			var channel = new ConsumerChannel<int>(new SynchronousActionQueue(), x => { });
			var filter = new FilterChannel<int>(new SynchronousActionQueue(), channel, x => true);

			new ChannelVisitor().Visit(filter);
		}

		[Test]
		public void Should_capture_the_interval_channel()
		{
			var channel = new ConsumerChannel<ICollection<int>>(new SynchronousActionQueue(), x => { });
			var scheduler = new TimerActionScheduler(new SynchronousActionQueue());
			var interval = new IntervalChannel<int>(new SynchronousActionQueue(), scheduler, 5.Minutes(), channel);

			new ChannelVisitor().Visit(interval);
		}

		[Test]
		public void Should_capture_the_instance_channel()
		{
			var provider = new DelegateChannelProvider<int>(x => new ConsumerChannel<int>(new SynchronousActionQueue(), y => { }));
			var channel = new InstanceChannel<int>(provider);

			new ChannelVisitor().Visit(channel);
		}

		[Test]
		public void Should_capture_the_instance_channel_with_thread_provider()
		{
			var provider = new DelegateChannelProvider<int>(x => new ConsumerChannel<int>(new SynchronousActionQueue(), y => { }));
			var threadProvider = new ThreadStaticChannelProvider<int>(provider);
			var channel = new InstanceChannel<int>(threadProvider);

			new ChannelVisitor().Visit(channel);
		}

		[Test]
		public void Should_capture_the_async_result_channel_and_state()
		{
			var channel = new AsyncResultChannel<int>(new ConsumerChannel<int>(new SynchronousActionQueue(), y => { }), x => { }, 0);

			new ChannelVisitor().Visit(channel);

			channel.Send(0);

			new ChannelVisitor().Visit(channel);
		}
	}
}
