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
	using System.Linq;
	using Magnum.TestFramework;
	using Stact.Data;


	[Scenario]
	public class Creating_an_ordered_sequence_from_unordered_values
	{
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
			InsertAndCompare(47, 27, 13, 11);
		}

		[Then, NotYetImplemented("Need to split single digit suffix")]
		public void Should_work_for_mixed_values_of_order()
		{
			InsertAndCompare(47, 27, 13, 69);
		}

		void InsertAndCompare(params int[] args)
		{
			var seq = OrderedSequence<int, int>.Empty(Monoid.IntAdd, x => x);
			for (int i = 0; i < args.Length; i++)
			{
				seq = seq.Insert(args[i]);
			}

			int[] values = seq.ToArray();
			values.Length.ShouldEqual(args.Length);

			var expected = args.OrderBy(x => x).ToArray();
			for (int i = 0; i < values.Length; i++)
			{
				values[i].ShouldEqual(expected[i]);
			}
		}
	}
}