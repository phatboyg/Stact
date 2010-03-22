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
namespace Magnum.Specs.Actions
{
	using System;
	using System.Diagnostics;
	using DateTimeExtensions;
	using Magnum.Actions;
	using Magnum.Actors;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Scheduling_an_action_for_now
	{
		[Test]
		public void Should_run_the_action_immediately()
		{
			ActionQueue queue = new ThreadPoolActionQueue();
			ActionScheduler scheduler = new TimerActionScheduler();

			var called = new Future<bool>();

			scheduler.Schedule(TimeSpan.Zero, queue, () => called.Complete(true));

			called.IsAvailable(1.Seconds()).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Scheduling_an_action_for_later
	{
		[Test]
		public void Should_wait_until_the_appropriate_time_for_the_action_to_execute()
		{
			ActionQueue queue = new ThreadPoolActionQueue();
			ActionScheduler scheduler = new TimerActionScheduler();

			var called = new Future<bool>();

			scheduler.Schedule(100.Milliseconds(), queue, () => called.Complete(true));

			called.IsAvailable(0.Seconds()).ShouldBeFalse();

			called.IsAvailable(1.Seconds()).ShouldBeTrue();
		}
	}


	[TestFixture]
	public class Scheduling_an_immediate_action_while_others_are_waiting
	{
		[Test]
		public void Should_run_the_new_action_immediately()
		{
			ActionQueue queue = new ThreadPoolActionQueue();
			ActionScheduler scheduler = new TimerActionScheduler();

			var called = new Future<bool>();

			scheduler.Schedule(2.Seconds(), queue, () => { });
			scheduler.Schedule(0.Seconds(), queue, () => called.Complete(true));

			called.IsAvailable(1.Seconds()).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Disabling_the_scheduler
	{
		[Test]
		public void Should_not_run_any_pending_actions()
		{
			ActionQueue queue = new ThreadPoolActionQueue();
			ActionScheduler scheduler = new TimerActionScheduler();

			var called = new Future<bool>();

			scheduler.Schedule(1.Seconds(), queue, () => called.Complete(true));

			scheduler.Disable();

			called.IsAvailable(2.Seconds()).ShouldBeFalse();
		}
	}

	[TestFixture]
	public class A_scheduled_item_throwing_an_exception
	{
		[Test]
		public void Should_not_stall_the_scheduler()
		{
			ActionQueue queue = new ThreadPoolActionQueue();
			ActionScheduler scheduler = new TimerActionScheduler();

			var called = new Future<bool>();

			scheduler.Schedule(200.Milliseconds(), queue, () => { throw new InvalidOperationException("Bugger!"); });
			scheduler.Schedule(400.Milliseconds(), queue, () => called.Complete(true));

			called.IsAvailable(1.Seconds()).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Scheduling_a_periodic_event
	{
		[Test]
		public void Should_run_the_action_until_disabled()
		{
			ActionQueue queue = new ThreadPoolActionQueue();
			ActionScheduler scheduler = new TimerActionScheduler();

			Stopwatch elapsed = Stopwatch.StartNew();

			int count = 0;
			var called = new Future<int>();
			var failed = new Future<bool>();

			ScheduledAction scheduledAction = null;
			scheduledAction = scheduler.Schedule(TimeSpan.Zero, 100.Milliseconds(), queue, () =>
				{
					count++;
					if (count == 10)
					{
						called.Complete(count);
						scheduledAction.Cancel();
					}
					else if (count > 10)
					{
						failed.Complete(true);
					}
				});

			called.IsAvailable(2.Seconds()).ShouldBeTrue();

			elapsed.Stop();

			failed.IsAvailable(200.Milliseconds()).ShouldBeFalse();

			Trace.WriteLine("Time Period: " + elapsed.ElapsedMilliseconds);
		}
	}
}