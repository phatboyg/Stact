namespace Magnum.ActorModel.Specs
{
	using System;
	using System.Threading;
	using Channels;
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

			bool consumed = false;

			channel.Subscribe(queue, message => consumed = true, message => message.LastActivity > DateTime.Now);

			var result = channel.Publish(update);

			Assert.IsTrue(result);

			Thread.Sleep(1000);

			Assert.IsFalse(consumed);
		}
	}

	public class UserUpdate
	{
		public DateTime LastActivity { get; set; }
	}
}