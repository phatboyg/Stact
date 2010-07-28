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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class When_sending_actions_back_into_itself
	{
		private static class SuperSleeper
		{
			private static long _ticksPerMs = Stopwatch.Frequency/1000;

			public static void Wait(long ms)
			{
				long finishAt = Stopwatch.GetTimestamp() + ms*_ticksPerMs;
				while (Stopwatch.GetTimestamp() < finishAt)
				{
				}
			}
		}

		[Test, Category("Slow"), Explicit]
		public void Should_properly_release_one_waiting_writer()
		{
			const int writerCount = 10;
			const int messageCount = 1000;

			var complete = new Future<bool>();
			int total = 0;

			Fiber reader = new ThreadFiber();
			
			try
			{
				Thread.Sleep(100);

				Stopwatch timer = Stopwatch.StartNew();

				var writers = new List<Fiber>();
				for (int i = 0; i < writerCount; i++)
				{
					Fiber fiber = new ThreadPoolFiber();
					for (int j = 0; j < messageCount; j++)
					{
						fiber.Add(() =>
							{
								SuperSleeper.Wait(1);

								reader.Add(() =>
									{
										total++;
										if (total == writerCount*messageCount)
											complete.Complete(true);
									});
							});
					}

					writers.Add(fiber);
				}

				complete.WaitUntilCompleted(20.Seconds()).ShouldBeTrue();

				timer.Stop();

				Trace.WriteLine("Elapsed time: " + timer.ElapsedMilliseconds + "ms (expected " + writerCount*messageCount + ")");
			}
			finally
			{
				reader.Stop();
			}
		}
	}
}