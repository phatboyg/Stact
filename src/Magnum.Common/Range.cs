namespace Magnum.Common
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a range of enumerable items
	/// </summary>
	/// <typeparam name="T">The type of range</typeparam>
	public class Range<T>
	{
		private readonly IComparer<T> _comparer;
		private readonly bool _includeLowerBound;
		private readonly bool _includeUpperBound;
		private readonly T _lowerBound;
		private readonly T _upperBound;

		/// <summary>
		/// Initializes a new Range
		/// </summary>
		/// <param name="lowerBound">The lower bound of the range</param>
		/// <param name="upperBound">The upper bound of the range</param>
		/// <param name="includeLowerBound">If the lower bound should be included</param>
		/// <param name="includeUpperBound">If the upper bound should be included</param>
		public Range(T lowerBound, T upperBound, bool includeLowerBound, bool includeUpperBound)
			: this(lowerBound, upperBound, includeLowerBound, includeUpperBound, Comparer<T>.Default)
		{
		}

		/// <summary>
		/// Initializes a new Range
		/// </summary>
		/// <param name="lowerBound">The lower bound of the range</param>
		/// <param name="upperBound">The upper bound of the range</param>
		/// <param name="includeLowerBound">If the lower bound should be included</param>
		/// <param name="includeUpperBound">If the upper bound should be included</param>
		/// <param name="comparer">The comparison to use for the range elements</param>
		private Range(T lowerBound, T upperBound, bool includeLowerBound, bool includeUpperBound, IComparer<T> comparer)
		{
			_comparer = comparer;
			_lowerBound = lowerBound;
			_upperBound = upperBound;
			_includeLowerBound = includeLowerBound;
			_includeUpperBound = includeUpperBound;
		}

		/// <summary>
		/// The lower bound of the range
		/// </summary>
		public T LowerBound
		{
			get { return _lowerBound; }
		}

		/// <summary>
		/// The upper bound of the range
		/// </summary>
		public T UpperBound
		{
			get { return _upperBound; }
		}

		/// <summary>
		/// The comparison used for the elements in the range
		/// </summary>
		public IComparer<T> Comparer
		{
			get { return _comparer; }
		}

		/// <summary>
		/// If the lower bound is included in the range
		/// </summary>
		public bool IncludeLowerBound
		{
			get { return _includeLowerBound; }
		}

		/// <summary>
		/// If the upper bound is included in the range
		/// </summary>
		public bool IncludeUpperBound
		{
			get { return _includeUpperBound; }
		}

		/// <summary>
		/// Determines if the value specified is contained within the range
		/// </summary>
		/// <param name="value">The value to check</param>
		/// <returns>Returns true if the value is contained within the range, otherwise false</returns>
		public bool Contains(T value)
		{
			int left = _comparer.Compare(value, _lowerBound);
			if (_comparer.Compare(value, _lowerBound) < 0 || (left == 0 && !_includeLowerBound))
			{
				return false;
			}

			int right = _comparer.Compare(value, _upperBound);
			return right < 0 || (right == 0 && _includeUpperBound);
		}

		/// <summary>
		/// Returns a forward enumerator for the range
		/// </summary>
		/// <param name="step">A function used to step through the range</param>
		/// <returns>An enumerator for the range</returns>
		public RangeEnumerator<T> Forward(Func<T, T> step)
		{
			return new RangeEnumerator<T>(this, step);
		}
	}
}