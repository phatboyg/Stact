namespace Magnum.Common.Specs.Threading
{
	using System;
	using System.Threading;
	using DateTimeExtensions;
	using MbUnit.Framework;

	[TestFixture]
	public class DeferFor_Specs
	{
		private ManualResetEvent _event;

		[SetUp]
		public void Setup()
		{
			_event = new ManualResetEvent(false);
		}

		[Test]
		public void The_method_should_be_called_when_the_defer_time_is_elapsed()
		{
			Action asyncAction = () => _event.Set();
			asyncAction.DeferFor(2.Seconds());

			Assert.IsFalse(_event.WaitOne(1.Seconds(), true), "should not have been set yet");

			Assert.IsTrue(_event.WaitOne(3.Seconds(), true), "Timeout waiting for event to be set");
		}

	}

	public static class ActionExtensions
	{
		public static void DeferFor(this Action action, TimeSpan timeSpan)
		{
			ThreadStart scheduledAction = () =>
			{
				Thread.Sleep(timeSpan);
				action();
			};

			Thread thread = new Thread(scheduledAction);
			thread.IsBackground = true;
			thread.Start();
		}
	}
}