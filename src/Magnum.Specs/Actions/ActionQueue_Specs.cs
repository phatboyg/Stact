namespace Magnum.Specs.Actions
{
	using System.Threading;
	using DateTimeExtensions;
	using Magnum.Actions;
	using Magnum.Actors;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Disabling_the_action_queue
	{
		[Test]
		public void Should_prevent_new_actions_from_being_queued()
		{
			ActionQueue queue = new ThreadPoolActionQueue();

			var called = new Future<bool>();

			queue.Disable();

			queue.Enqueue(() => called.Complete(true));

			bool completed = queue.RunAll(1.Seconds());

			completed.ShouldBeTrue();

			called.IsAvailable().ShouldBeFalse();
		}
	}

	[TestFixture]
	public class Running_all_actions
	{
		[Test]
		public void Should_result_in_no_waiting_actions_in_the_queue()
		{
			ActionQueue queue = new ThreadPoolActionQueue();

			queue.Enqueue(() => Thread.Sleep(1000));

			var called = new Future<bool>();

			queue.Enqueue(() => called.Complete(true));

			bool completed = queue.RunAll(2.Seconds());

			completed.ShouldBeTrue();

			called.IsAvailable().ShouldBeTrue();
		}
	}
}
