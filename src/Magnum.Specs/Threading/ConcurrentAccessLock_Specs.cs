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
namespace Magnum.Specs.Threading
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using DateTimeExtensions;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;
	using Magnum.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class ConcurrentAccessLock_Specs
	{
		[Test]
		public void Should_only_allow_the_specified_number_of_threads_through_at_one_time()
		{
			bool called = false;

			var accessLock = new SemaphoreAccessLock(1);

			accessLock.Execute(() => called = true, 5.Seconds());

			called.ShouldBeTrue();
		}

		[Test]
		public void Should_block_a_thread_if_another_is_running()
		{
			var called = new Future<bool>();

			bool failed = false;

			var accessLock = new SemaphoreAccessLock(1);

			CommandQueue queue = new ThreadPoolCommandQueue();
			queue.Enqueue(() => accessLock.Execute(() =>
				{
					called.Complete(true);
					Thread.Sleep(1000);
				}, 5.Seconds()));
			;

			called.IsAvailable(3.Seconds()).ShouldBeTrue();

			try
			{
				accessLock.Execute(() => failed = true, 0.Seconds());

				Assert.Fail("Should have thrown a timeout exception");
			}
			catch (TimeoutException)
			{
			}
			catch (Exception ex)
			{
				Assert.Fail("Unknown excpetion thrown: " + ex);
			}

			failed.ShouldBeFalse();

			accessLock.Execute(() => failed = true, 5.Seconds());

			failed.ShouldBeTrue();
		}

		[Test]
		public void Using_two_locks_should_contain_a_thread_pool()
		{
			// first, we want an access lock on the queue itself for reading

			// second, we want an access lock to manage how many threads we have running at one time
			
			// third we need a feeder that is going to pick up a worker thread and push content through it

			AccessLock queueLock = new SemaphoreAccessLock(1);
			AccessLock threadLock = new SemaphoreAccessLock(4);

			CommandQueue threadQueue = new ThreadPoolCommandQueue();

			int count = 0;
			int queueCount = 1;

			Future<bool> called = new Future<bool>();

			Action<CommandQueue> reader = null;
			reader = q =>
				{
					try
					{
						threadLock.Execute(() =>
							{
								try
								{
									queueLock.Execute(() =>
										{
											try
											{
												threadLock.Execute(() =>
													{
														CommandQueue subQueue = new ThreadPoolCommandQueue();
														subQueue.Enqueue(() => reader(subQueue));
														Interlocked.Increment(ref queueCount);
														
													}, 0.Seconds());
											}
											catch (Exception)
											{
												throw;
											}

											Thread.Sleep(200);

											if(Interlocked.Increment(ref count) == 100)
												called.Complete(true);

										}, 500.Milliseconds());

									Thread.Sleep(300);

									q.Enqueue(() => reader(q));
								}
								catch
								{
									q.Disable();
									Interlocked.Decrement(ref queueCount);
								}
							
							}, 1000.Milliseconds());
					}
					catch
					{
						q.Disable();
						Interlocked.Decrement(ref queueCount);
					}
				};

			Stopwatch sw = Stopwatch.StartNew();

			threadQueue.Enqueue(() => reader(threadQueue));

			called.IsAvailable(3110.Seconds()).ShouldBeTrue();
			sw.Stop();

			Trace.WriteLine("Time: " + sw.ElapsedMilliseconds.ToString());
			Trace.WriteLine("Count: " + queueCount);
		}
	}
}