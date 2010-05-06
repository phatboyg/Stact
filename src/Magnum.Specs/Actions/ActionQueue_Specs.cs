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
	using System.Threading;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using Magnum.Logging;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestFramework;

	[TestFixture]
	public class Disabling_the_action_queue
	{
		[Test]
		public void Should_prevent_new_actions_from_being_queued()
		{
			Fiber fiber = new ThreadPoolFiber();

			var called = new Future<bool>();

			fiber.StopAcceptingActions();

			Assert.Throws<FiberException>(() => fiber.Enqueue(() => called.Complete(true)));

			fiber.ExecuteAll(10.Seconds());

			called.IsCompleted.ShouldBeFalse();
		}
	}

	[TestFixture]
	public class Running_all_actions
	{
		[Test]
		public void Should_result_in_no_waiting_actions_in_the_queue()
		{
			TraceLogProvider.Configure(LogLevel.Debug);

			Fiber fiber = new ThreadPoolFiber();

			var called = new Future<bool>();

			10.Times(() => fiber.Enqueue(() => Thread.Sleep(100)));
			fiber.Enqueue(() => called.Complete(true));

			Stopwatch timer = Stopwatch.StartNew();

			fiber.ExecuteAll(8.Seconds());

			timer.Stop();

			called.IsCompleted.ShouldBeTrue();

			timer.ElapsedMilliseconds.ShouldBeLessThan(2000);
		}
	}

	[TestFixture]
	public class Adding_an_action_to_a_full_queue
	{
		[Test]
		public void Should_result_in_an_exception()
		{
			var action = MockRepository.GenerateMock<Action>();

			var queue = new ThreadPoolFiber(2, 0);
			queue.Enqueue(action);
			queue.Enqueue(action);

			try
			{
				queue.Enqueue(action);
				Assert.Fail("Should have thrown an exception");
			}
			catch (FiberException ex)
			{
				ex.Message.Contains("Insufficient").ShouldBeTrue();
			}
		}
	}

	[TestFixture]
	public class Running_all_actions_using_a_thread_queue
	{
		[Test]
		public void Should_result_in_no_waiting_actions_in_the_queue()
		{
			Fiber fiber = new ThreadFiber();

			fiber.Enqueue(() => Thread.Sleep(1000));

			var called = new Future<bool>();

			fiber.Enqueue(() => called.Complete(true));

			fiber.ExecuteAll(12.Seconds());

			called.IsCompleted.ShouldBeTrue();
		}
	}
}