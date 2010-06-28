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
namespace Magnum.Specs.Concurrency
{
	using System.Diagnostics;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Concurrency;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;


	[TestFixture]
	public class Creating_a_sizeable_load
	{
		[Test, Category("Slow")]
		public void Should_work()
		{
			Stopwatch timer = Stopwatch.StartNew();

			FiberFactory fiberFactory = () => new ThreadPoolFiber();

			const int channelCount = 10000;
			const int seedCount = 500;

			var channels = new Channel<string>[channelCount];

			var completed = new Future<int>();

			var latch = new CountDownLatch(channelCount*seedCount, completed.Complete);

			for (int i = 0; i < channelCount; i++)
			{
				int channelNumber = i;
				channels[i] = new ConsumerChannel<string>(fiberFactory(), x =>
					{
						if (channelNumber < channels.Length - 1)
							channels[channelNumber + 1].Send(x);

						latch.CountDown();
					});
			}

			for (int i = 0; i < seedCount; i++)
			{
				channels[i].Send("Hi");

				for (int j = 0; j < i; j++)
					latch.CountDown();
			}

			completed.WaitUntilCompleted(24.Seconds()).ShouldBeTrue();

			timer.Stop();

			Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
		}
	}
}