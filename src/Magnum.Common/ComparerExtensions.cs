namespace Magnum.Common
{
	using System.Collections.Generic;

	public static class ComparerExtensions
	{
		public static IComparer<T> Reverse<T>(this IComparer<T> original)
		{
			ReverseComparer<T> originalAsReverse = original as ReverseComparer<T>;
			if (originalAsReverse != null)
				return originalAsReverse.OriginalComparer;

			return new ReverseComparer<T>(original);
		}

		public static IComparer<T> ThenBy<T>(this IComparer<T> firstComparer, IComparer<T> secondComparer)
		{
			return new LinkedComparer<T>(firstComparer, secondComparer);
		}
	}
}