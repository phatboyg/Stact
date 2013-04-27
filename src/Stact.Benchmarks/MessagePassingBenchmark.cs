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
namespace Stact.Benchmarks
{
	using System;
	using System.Diagnostics;
	using Magnum.Concurrency;
	using Magnum.Extensions;


    public class MessagePassingBenchmark
	{
		public void Run()
		{
			Run(() => "Hi");
			Run(() => 47);
		}

		void Run<T>(Func<T> valueProvider)
		{
			Stopwatch timer = Stopwatch.StartNew();

			const int channelCount = 10000;
			const int seedCount = 500;

			var complete = new Future<int>();

			bool completed = RunTest(channelCount, seedCount, complete, valueProvider);

			timer.Stop();

			if (!completed)
			{
				Console.WriteLine("Process did not complete");
				return;
			}

			Console.WriteLine("Channel<{0}> Benchmark", typeof(T).Name);

			Console.WriteLine("Processed {0} messages in with {1} channels in {2}ms", seedCount, channelCount,
			                  timer.ElapsedMilliseconds);

			Console.WriteLine("That's {0} messages per second!", ((long)seedCount*channelCount*1000)/timer.ElapsedMilliseconds);
		}

		bool RunTest<T>(int channelCount, int seedCount, Future<int> complete, Func<T> valueProvider)
		{
			var channels = new Channel<T>[channelCount];

			var latch = new CountdownLatch(channelCount*seedCount, complete.Complete);

			FiberFactory fiberFactory = () => new TaskFiber();

			for (int i = 0; i < channelCount; i++)
			{
				int channelNumber = i;
				channels[i] = new ConsumerChannel<T>(fiberFactory(), x =>
				{
					if (channelNumber < channels.Length - 1)
						channels[channelNumber + 1].Send(x);

					latch.CountDown();
				});
			}

			for (int i = 0; i < seedCount; i++)
			{
				channels[i].Send(valueProvider());

				for (int j = 0; j < i; j++)
					latch.CountDown();
			}

			return complete.WaitUntilCompleted(24.Seconds());
		}
	}
}