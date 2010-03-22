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

			bool completed = queue.WaitAll(1.Seconds());

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

			bool completed = queue.WaitAll(2.Seconds());

			completed.ShouldBeTrue();

			called.IsAvailable().ShouldBeTrue();
		}
	}

	[TestFixture]
	public class Running_all_actions_using_a_thread_queue
	{
		[Test]
		public void Should_result_in_no_waiting_actions_in_the_queue()
		{
			ActionQueue queue = new ThreadActionQueue();

			queue.Enqueue(() => Thread.Sleep(1000));

			var called = new Future<bool>();

			queue.Enqueue(() => called.Complete(true));

			bool completed = queue.WaitAll(2.Seconds());

			completed.ShouldBeTrue();

			called.IsAvailable().ShouldBeTrue();
		}
	}
}