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
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;

	[TestFixture]
	public class Publishing_a_message_on_a_channel
	{
		[Test]
		public void Should_filter_out_unwanted_messages()
		{
			var update = new UserUpdate {LastActivity = DateTime.Now - 5.Minutes()};

			Fiber fiber = new SynchronousFiber();

			var future = new FutureChannel<UserUpdate>();

			var filter = new FilterChannel<UserUpdate>(fiber, future, x => x.LastActivity > DateTime.Now);

			Channel<UserUpdate> channel = new BroadcastChannel<UserUpdate>(new[] {filter});

			channel.Send(update);

			Assert.IsFalse(future.WaitUntilCompleted(1.Seconds()));
		}

		[Test]
		public void Should_return_false_if_there_are_no_subscribers()
		{
			Fiber fiber = new SynchronousFiber();

			Channel<UserUpdate> channel = new BroadcastChannel<UserUpdate>(new Channel<UserUpdate>[] {});

			var update = new UserUpdate();

			channel.Send(update);

			// exception here? or just ignore
		}

		[Test]
		public void Should_schedule_events()
		{
			var update = new UserUpdate {LastActivity = DateTime.Now - 5.Minutes()};

			Fiber fiber = new SynchronousFiber();

			var future = new FutureChannel<UserUpdate>();

			Channel<UserUpdate> channel = new BroadcastChannel<UserUpdate>(new Channel<UserUpdate>[] {future});

			var scheduler = new TimerScheduler(fiber);

			scheduler.Schedule(1000, fiber, () => channel.Send(update));

			Thread.Sleep(500);

			Assert.IsFalse(future.WaitUntilCompleted(0.Seconds()));

			Assert.IsTrue(future.WaitUntilCompleted(1.Seconds()));
		}
	}

	public class UserUpdate
	{
		public DateTime LastActivity { get; set; }
	}
}