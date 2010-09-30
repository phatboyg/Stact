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
namespace Stact.Specs.Channels
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using Fibers;
	using Stact.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using Magnum.TestFramework;

	[TestFixture]
	public class Using_a_thread_pool_of_channels
	{
		private class TestMessage
		{
			public Action Complete;

			public TestMessage(Action complete)
			{
				Complete = complete;
			}
		}

		[Test, Category("Slow"), Explicit]
		public void Should_not_allow_more_than_the_specified_number_of_consumers()
		{
			int count = 0;

			var exceeded = new Future<int>();
			var complete = new Future<bool>();

			var locker = new object();

			int limit = 20;
			var provider = new DelegateChannelProvider<TestMessage>(m =>
				{
					var consumer = new ConsumerChannel<TestMessage>(new SynchronousFiber(), message =>
						{
							int value = Interlocked.Increment(ref count);
							Trace.WriteLine("Value: " + value);
							if (value > limit)
								exceeded.Complete(value);

							message.Complete();

							lock (locker)
							{
								while (complete.IsCompleted == false)
								{
									Monitor.Wait(locker);
								}
							}

							Interlocked.Decrement(ref count);
						});

					return consumer;
				});

			Channel<TestMessage> channel = new ThreadPoolChannel<TestMessage>(provider, limit);

			for (int i = 0; i < limit; i++)
			{
				channel.Send(new TestMessage(() => { }));
			}

			lock (locker)
			{
				while (count < limit)
				{
					Monitor.Wait(locker, 100);
				}
			}

			Trace.WriteLine("Sending extra message to try and break it");

			ThreadPool.QueueUserWorkItem(x =>
			                             channel.Send(new TestMessage(() =>
			                             	{
			                             		lock (locker)
			                             		{
			                             			complete.Complete(true);
			                             			Monitor.PulseAll(locker);
			                             		}
			                             	})));

			lock (locker)
			{
				Trace.WriteLine("Waiting for message to get executed");
				bool ready = Monitor.Wait(locker, 5.Seconds());

				// it should not, since the queue is full;
				ready.ShouldBeFalse();
			}

			Trace.WriteLine("Marking complete");
			complete.Complete(true);

			exceeded.IsCompleted.ShouldBeFalse();
		}
	}
}