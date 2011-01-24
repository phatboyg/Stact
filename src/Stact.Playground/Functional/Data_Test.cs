namespace Stact.Functional
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using Data.Internal;
	using Stact.Data;


	public class Data_Test
	{
		void Test_Sequence()
		{
			var random = new Random();
			int[] values = Enumerable.Range(0, 1000)
				.Select(x => random.Next(0, 1000))
				.ToArray();

			Sequence<int> seq = Sequence<int>.Empty();
			for (int i = 0; i < values.Length; i++)
			{
				seq = seq.PushBack(values[i]);
			}

			Trace.WriteLine("Count: " + seq.Count);

			StringBuilder sb = new StringBuilder(":> ");

			seq.Visit(x =>
				{
					sb.Append(x).Append(' ');
					return true;
				});

			Trace.WriteLine(sb.ToString());
		}

		void What_is_cons()
		{
			var seq = Sequence<int>.Single(5).PushFront(10);
			seq.Visit(x =>
				{
					Trace.WriteLine(x);
					return true;
				});
		}

		void Test_order()
		{
			Data.OrderedSequence<int, int> seq = Stact.Data.OrderedSequence<int,int>.Empty(Monoid.IntAdd, x => x);

			seq = seq.Insert(27);
			seq = seq.Insert(42);
			seq = seq.Insert(13);
			seq = seq.Insert(22);

			seq.Visit(value =>
				{
					Trace.WriteLine(value);
					return true;
				});
		}
	}
}