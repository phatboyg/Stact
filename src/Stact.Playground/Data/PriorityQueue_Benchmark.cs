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
namespace Stact.Data
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using FingerTree;
	using Magnum.Extensions;


	public class Given_a_priority_queue
	{
		void BenchmarkQueue()
		{
			Benchmark(typeof(Queue<int>), StandardQueue, 10, 10000000);
			Benchmark(typeof(FNSeq<int>), FunctionalSequence, 10, 10000000);
		}

		void Benchmark(Type t, Action<int> test, params int[] limits)
		{
			const int loops = 3;
			var results = new long[loops];

			for (int index = 0; index < limits.Length; index++)
			{
				int limit = limits[index];

				for (int loop = 0; loop < loops; loop++)
				{
					Stopwatch timer = Stopwatch.StartNew();

					StandardQueue(limit);

					timer.Stop();
					results[loop] = timer.ElapsedMilliseconds;
				}

				if (limit >= 100)
				{
					double average = results.Average();

					Trace.WriteLine("Queue<int>({0}): {1:0}ms".FormatWith(limit, average));
				}
			}
		}

		static void StandardQueue(int limit)
		{
			var q = new Queue<int>();

			for (int i = 0; i < limit; i++)
				q.Enqueue(i);

			while (q.Count > 0)
				q.Dequeue();
		}

		static void FunctionalSequence(int limit)
		{
			var q = new FNSeq<int>(Enumerable.Range(0, limit));

			while (q.Length() > 0)
				q = q.remove(0);

		}
	}
}