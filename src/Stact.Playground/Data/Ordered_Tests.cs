namespace Stact.Data
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Internal;


	public class An_ordered_sequence
	{
		void Should_put_things_in_order()
		{
			var random = new Random();
			int[] values = Enumerable.Range(0, 10000)
				.Select(x => random.Next(0, 10000))
				.ToArray();

			//int[] values = {47, 37, 27, 21, 42, 29, 13, 17, 7, 1, 0, -42};

			var sequence = new Stact.Data.Internal.OrderedSequence<int, int>(new Key<int, int>(x => x), values);
			Stopwatch first = Stopwatch.StartNew();
			sequence = new Stact.Data.Internal.OrderedSequence<int, int>(new Key<int, int>(x => x), values);
			first.Stop();

			Stopwatch second = Stopwatch.StartNew();
			IEnumerable<int> orderedEnumerable = values.OrderBy(x => x).ToArray();
			second.Stop();

			bool match = orderedEnumerable.SequenceEqual(sequence);

			Trace.WriteLine("Matched: " + match);
			Trace.WriteLine("First: " + first.ElapsedMilliseconds);
			Trace.WriteLine("Second: " + second.ElapsedMilliseconds);

		}
	}
}