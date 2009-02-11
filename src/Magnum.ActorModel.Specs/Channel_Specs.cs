namespace Magnum.ActorModel.Specs
{
	using System;
	using System.Threading;
	using Channels;
	using CommandQueues;
	using DateTimeExtensions;
	using NUnit.Framework;

	[TestFixture]
	public class Publishing_a_message_on_a_channel
	{
		[Test]
		public void Should_return_false_if_there_are_no_subscribers()
		{
			Channel<UserUpdate> channel = new ChannelImpl<UserUpdate>();

			UserUpdate update = new UserUpdate();

			var result = channel.Publish(update);

			Assert.IsFalse(result);
		}

		[Test]
		public void Should_filter_out_unwanted_messages()
		{
			Channel<UserUpdate> channel = new ChannelImpl<UserUpdate>();

			UserUpdate update = new UserUpdate {LastActivity = DateTime.Now - 5.Minutes()};

			CommandQueue queue = new SynchronousCommandQueue();

			var future = new Future<UserUpdate>();

			channel.Subscribe(queue, future.Complete, message => message.LastActivity > DateTime.Now);

			var result = channel.Publish(update);
			Assert.IsTrue(result);

			Assert.IsFalse(future.IsAvailable(1.Seconds()));
		}

		[Test]
		public void Should_schedule_events()
		{
			Channel<UserUpdate> channel = new ChannelImpl<UserUpdate>();

			var update = new UserUpdate {LastActivity = DateTime.Now - 5.Minutes()};

			CommandQueue queue = new SynchronousCommandQueue();

			var context = new ThreadCommandContext(queue);

			var future = new Future<UserUpdate>();

			channel.Subscribe(queue, future.Complete);

			context.Schedule(1000, () => channel.Publish(update));

			Thread.Sleep(500);

			Assert.IsFalse(future.IsAvailable(0.Seconds()));

			Assert.IsTrue(future.IsAvailable(1.Seconds()));
		}
	}

	public class UserUpdate
	{
		public DateTime LastActivity { get; set; }
	}
}