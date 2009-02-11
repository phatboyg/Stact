namespace Magnum.ActorModel.Specs
{
	using System;
	using System.Threading;
	using CommandQueues;
	using NUnit.Framework;
	using Schedulers;

	[TestFixture]
	public class ThreadActionScheduler_Specs
	{
		[Test]
		public void Schedule()
		{
			var queue = new SynchronousCommandQueue();

			var count = 0;
			var reset = new AutoResetEvent(false);
			Action one = () => Assert.AreEqual(0, count++);
			Action two = () => Assert.AreEqual(1, count++);
			Action three = delegate
				{
					Assert.AreEqual(2, count++);
					reset.Set();
				};

			using (var thread = new ThreadActionScheduler())
			{
				thread.Schedule(50, () => queue.Enqueue(three));
				thread.Schedule(1, () => queue.Enqueue(one));
				thread.Schedule(1, () => queue.Enqueue(two));
				Assert.IsTrue(reset.WaitOne(10000, false));
			}
		}

		[Test]
		public void Schedule1000In1ms()
		{
			var queue = new SynchronousCommandQueue();
			queue.Run();

			var count = 0;
			var reset = new AutoResetEvent(false);
			Action one = delegate
				{
					count++;
					if (count == 1000)
					{
						reset.Set();
					}
				};

			using (var thread = new ThreadActionScheduler())
			{
				for (var i = 0; i < 1000; i++)
				{
					thread.Schedule(i, () => queue.Enqueue(one));
				}
				Assert.IsTrue(reset.WaitOne(1200, false));
			}
		}

		[Test]
		public void TimeTilNext()
		{
			var queue = new SynchronousCommandQueue();
			queue.Run();
			Action action = () => Assert.Fail("Should not execute");
			using (var timer = new ThreadActionScheduler())
			{
				long now = 0;
				long span = 0;
				timer.QueueEvent(new SingleEvent(500, () => queue.Enqueue(action), now));
				Assert.IsTrue(timer.GetNextScheduledTime(ref span, 0));
				Assert.AreEqual(500, span);
				Assert.IsTrue(timer.GetNextScheduledTime(ref span, 499));
				Assert.AreEqual(1, span);
				Assert.IsFalse(timer.GetNextScheduledTime(ref span, 500));
				Assert.AreEqual(0, span);
			}
		}

		[Test]
		public void TimeTilNextNothingQueued()
		{
			using (var timer = new ThreadActionScheduler())
			{
				long result = 0;
				Assert.IsFalse(timer.GetNextScheduledTime(ref result, 100));
				Assert.AreEqual(0, result);
			}
		}
	}
}