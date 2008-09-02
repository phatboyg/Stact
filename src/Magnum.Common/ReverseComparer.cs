using System.Collections.Generic;

namespace Magnum.Common
{
	public class ReverseComparer<T> : IComparer<T>
	{
		private readonly IComparer<T> _originalComparer;

		public IComparer<T> OriginalComparer
		{
			get { return _originalComparer; }
		}

		public ReverseComparer(IComparer<T> original)
		{
			original.MustNotBeNull("original");

			_originalComparer = original;
		}

		public int Compare(T x, T y)
		{
			return _originalComparer.Compare(y, x);
		}
	}
}