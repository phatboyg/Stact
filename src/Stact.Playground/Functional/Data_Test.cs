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
		}

	}
}