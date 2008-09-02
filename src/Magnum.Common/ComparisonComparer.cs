namespace Magnum.Common
{
	using System;
	using System.Collections.Generic;

	public class ComparisonComparer<T> : IComparer<T>
	{
		private readonly Comparison<T> _comparison;

		public ComparisonComparer(Comparison<T> comparison)
		{
			comparison.MustNotBeNull("comparison");

			_comparison = comparison;
		}

		public int Compare(T x, T y)
		{
			return _comparison(x, y);
		}

		public static Comparison<T> CreateComparison(IComparer<T> comparer)
		{
			comparer.MustNotBeNull("comparer");

			return (x, y) => comparer.Compare(x, y);
		}
	}
}