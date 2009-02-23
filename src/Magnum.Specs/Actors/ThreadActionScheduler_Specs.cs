// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Specs.Actors
{
	using System;
	using System.Threading;
	using Magnum.Actors.CommandQueues;
	using Magnum.Actors.Schedulers;
	using MbUnit.Framework;

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

			using (var thread = new ThreadPoolScheduler())
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

			using (var thread = new ThreadPoolScheduler())
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
			using (var timer = new ThreadPoolScheduler())
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
			using (var timer = new ThreadPoolScheduler())
			{
				long result = 0;
				Assert.IsFalse(timer.GetNextScheduledTime(ref result, 100));
				Assert.AreEqual(0, result);
			}
		}
	}
}