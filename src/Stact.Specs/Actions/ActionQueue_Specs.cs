// Copyright 2010 Chris Patterson
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
namespace Stact.Specs.Actions
{
	using System.Diagnostics;
	using System.Threading;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;


	[TestFixture]
	public class Disabling_the_action_queue
	{
		[Test]
		public void Should_prevent_new_actions_from_being_queued()
		{
			Fiber fiber = new PoolFiber();

			var called = new Future<bool>();

			fiber.Stop();

			fiber.Add(() => called.Complete(true));

			fiber.Shutdown(10.Seconds());

			called.IsCompleted.ShouldBeFalse();
		}
	}


	[TestFixture]
	public class Running_all_actions
	{
		[Test]
		public void Should_result_in_no_waiting_actions_in_the_queue()
		{
			Fiber fiber = new PoolFiber();

			var called = new Future<bool>();

			10.Times(() => fiber.Add(() => Thread.Sleep(100)));
			fiber.Add(() => called.Complete(true));

			Stopwatch timer = Stopwatch.StartNew();

			fiber.Shutdown(8.Seconds());

			timer.Stop();

			called.IsCompleted.ShouldBeTrue();

			timer.ElapsedMilliseconds.ShouldBeLessThan(2000);
		}
	}


	[TestFixture]
	public class Running_all_actions_using_a_thread_queue
	{
		[Test]
		[Category("Slow")]
		public void Should_result_in_no_waiting_actions_in_the_queue()
		{
			Fiber fiber = new ThreadFiber();

			fiber.Add(() => Thread.Sleep(1000));

			var called = new Future<bool>();

			fiber.Add(() => called.Complete(true));

			fiber.Shutdown(112.Seconds());

			called.IsCompleted.ShouldBeTrue();
		}
	}
}