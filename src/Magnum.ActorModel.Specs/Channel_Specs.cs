namespace Magnum.ActorModel.Specs
{
	using System;
	using Channels;
	using NUnit.Framework;

	[TestFixture]
	public class Publishing_a_message_on_a_channel
	{
		[Test]
		public void Should_return_false_if_there_are_no_subscribers()
		{
			Channel<UserUpdate> channel = new Channel<UserUpdate>();

			UserUpdate update = new UserUpdate();

			var result = channel.Publish(update);

			Assert.IsFalse(result);
		}
	}

	public class UserUpdate
	{
		public DateTime LastActivity { get; set; }
	}
}