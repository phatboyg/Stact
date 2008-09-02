namespace Magnum.Common
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class RangeEnumerator<T> :
		IEnumerable<T>
	{
		private readonly Range<T> _range;
		private readonly Func<T, T> _step;
		private readonly bool _ascending;

		public RangeEnumerator(Range<T> range, Func<T, T> step)
		{
			_range = range;
			_step = step;

			_ascending = range.Comparer.Compare(range.LowerBound, step(range.LowerBound)) < 0;
		}

		public IEnumerator<T> GetEnumerator()
		{
			T first = _ascending ? _range.LowerBound : _range.UpperBound;
			T last = _ascending ? _range.UpperBound : _range.LowerBound;

			T value = first;

			IComparer<T> comparer = _range.Comparer;

			if (_range.IncludeLowerBound)
			{
				if (_range.IncludeUpperBound || comparer.Compare(value, last) < 0)
					yield return value;
			}

			value = _step(value);

			while (comparer.Compare(value, last) < 0)
			{
				yield return value;
				value = _step(value);
			}

			if (_range.IncludeUpperBound && comparer.Compare(value, last) == 0)
				yield return value;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}