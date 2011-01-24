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
	using System.Linq;
	using Data;


	public class SlowAssFingerTreeBenchmark
	{
		Stopwatch _timer;

		public void Run()
		{
			RunTest(10);
			RunTest(10000);
		}

		void RunTest(int count)
		{
			var random = new Random();
			int[] values = Enumerable.Range(0, count)
				.Select(x => random.Next(0, 10000))
				.ToArray();

			_timer = Stopwatch.StartNew();
			OrderedSequence<int, int> seq = OrderedSequence<int, int>.Empty(Monoid.IntNext, x => x);
			for (int i = 0; i < values.Length; i++)
				seq = seq.Insert(values[i]);
			_timer.Stop();

			if(count >= 100)
				Console.WriteLine("Finger Tree Elapsed Time (" + count + "): " + _timer.ElapsedMilliseconds + "ms");
		}
	}
}