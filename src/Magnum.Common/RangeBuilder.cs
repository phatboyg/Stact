namespace Magnum.Common
{
	public class RangeBuilder<T>
	{
		internal bool _includeLowerBound;
		internal bool _includeUpperBound;
		internal T _lowerBound;
		internal T _upperBound;

		public RangeBuilder(T lowerBound)
		{
			_lowerBound = lowerBound;
			_upperBound = lowerBound;
			_includeLowerBound = true;
			_includeUpperBound = true;
		}

		/// <summary>
		/// Specifies the upper bound for the range
		/// </summary>
		/// <param name="upperBound"></param>
		/// <returns></returns>
		public RangeBuilder<T> Through(T upperBound)
		{
			_upperBound = upperBound;
			return this;
		}

		public RangeBuilder<T> IncludeLowerBound()
		{
			_includeLowerBound = true;
			return this;
		}

		public RangeBuilder<T> IncludeUpperBound()
		{
			_includeUpperBound = true;
			return this;
		}

		public RangeBuilder<T> ExcludeLowerBound()
		{
			_includeLowerBound = false;
			return this;
		}

		public RangeBuilder<T> ExcludeUpperBound()
		{
			_includeUpperBound = false;
			return this;
		}

		public static implicit operator Range<T>(RangeBuilder<T> builder)
		{
			return new Range<T>(builder._lowerBound, builder._upperBound, builder._includeLowerBound, builder._includeUpperBound);
		}
	}

	public static class RangeBuilderExt
	{
		public static RangeBuilder<T> Through<T>(this T start, T end)
		{
			return new RangeBuilder<T>(start).Through(end);
		}
	}
}