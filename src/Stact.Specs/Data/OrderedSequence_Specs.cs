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
namespace Stact.Specs.Data
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Magnum.TestFramework;
	using NUnit.Framework;
	/*


	[Scenario]
	public class Creating_an_ordered_sequence_from_unordered_values
	{
		Stopwatch _timer;

		[Then]
		public void Should_work_for_two_values()
		{
			InsertAndCompare(27, 13);
		}

		[Then]
		public void Should_work_for_three_values()
		{
			InsertAndCompare(47, 27, 13);
		}

		[Then]
		public void Should_work_for_four_values()
		{
			InsertAndCompare(47, 27, 13, 42);
		}

		[Then]
		public void Should_work_for_mixed_values_of_order()
		{
			InsertAndCompare(47, 27, 13, 69);
		}

		[Then]
		public void Should_work_for_five_values()
		{
			InsertAndCompare(47, 27, 13, 69, 72);
		}
		
		[Then]
		public void Should_work_for_six_values()
		{
			InsertAndCompare(47, 27, 13, 69, 72, 21);
		}
	
		[Then]
		public void Should_work_for_seven_values()
		{
			InsertAndCompare(47, 27, 13, 69, 72, 12, 21);
		}

		[Then]
		public void Should_work_for_eight_values()
		{
			InsertAndCompare(47, 27, 13, 69, 72, 1, 54, 9);
		}

		[Then]
		public void Should_handle_ordering_properly()
		{
			InsertAndCompare(10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 55, 45, 35, 25, 15, 5, 7, 8, 9, 110, 75);
		}

		[Then, Explicit]
		public void Should_handle_a_large_number_of_random_values()
		{
			var random = new Random();
			int[] values = Enumerable.Range(0, 10000)
				.Select(x => random.Next(0, 10000))
				.ToArray();

			InsertAndCompare(values);

			Trace.WriteLine("Elapsed Time: " + _timer.ElapsedMilliseconds + "ms");
		}			

		[Then, Explicit]
		public void Should_compare_to_a_hash_set_of_values()
		{
			var random = new Random();
			int[] values = Enumerable.Range(0, 10000)
				.Select(x => random.Next(0, 10000))
				.ToArray();

			HowAboutADictionary(values);

			Trace.WriteLine("Elapsed Time: " + _timer.ElapsedMilliseconds + "ms");
		}			

		void HowAboutADictionary(params int[] args)
		{
			_timer = Stopwatch.StartNew();
			var dictionary = new HashSet<int>(args);
			_timer.Stop();
		}

		void InsertAndCompare(params int[] args)
		{
			_timer = Stopwatch.StartNew();
			var seq = OrderedSequence<int, int>.Empty(Monoid.IntNext, x => x);
			for (int i = 0; i < args.Length; i++)
			{
				seq = seq.Insert(args[i]);
			}
			_timer.Stop();

			int[] values = seq.ToArray();
			if(values.Length != args.Length)
			{
				for (int j = 0; j < values.Length; j++)
				{
					Trace.Write(values[j] + " ");
				}
				Trace.WriteLine("");
			}
			values.Length.ShouldEqual(args.Length);

			var expected = args.OrderBy(x => x).ToArray();
			for (int i = 0; i < values.Length; i++)
			{
				values[i].ShouldEqual(expected[i]);
			}
		}
	}
	 */
}