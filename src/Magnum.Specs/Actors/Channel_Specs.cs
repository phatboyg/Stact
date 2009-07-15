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
	using DateTimeExtensions;
	using Magnum.Actors;
	using Magnum.Actors.Channels;
	using Magnum.Actors.CommandQueues;
	using Magnum.Actors.Schedulers;
	using NUnit.Framework;

	[TestFixture]
	public class Publishing_a_message_on_a_channel
	{
		[Test]
		public void Should_return_false_if_there_are_no_subscribers()
		{
			Channel<UserUpdate> channel = new SynchronousChannel<UserUpdate>();

			UserUpdate update = new UserUpdate();

			var result = channel.Publish(update);

			Assert.IsFalse(result);
		}

		[Test]
		public void Should_filter_out_unwanted_messages()
		{
			Channel<UserUpdate> channel = new SynchronousChannel<UserUpdate>();

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
			Channel<UserUpdate> channel = new SynchronousChannel<UserUpdate>();

			var update = new UserUpdate {LastActivity = DateTime.Now - 5.Minutes()};

			CommandQueue queue = new SynchronousCommandQueue();

			var scheduler = new ThreadPoolScheduler();

			var future = new Future<UserUpdate>();

			channel.Subscribe(queue, future.Complete);

			scheduler.Schedule(1000, () => channel.Publish(update));

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