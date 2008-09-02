namespace Magnum.Common
{
	using System.Collections.Generic;

	public class LinkedComparer<T> : IComparer<T>
	{
		private readonly IComparer<T> primary, secondary;

		public LinkedComparer(IComparer<T> primary, IComparer<T> secondary)
		{
			primary.MustNotBeNull("primary");
			secondary.MustNotBeNull("secondary");

			this.primary = primary;
			this.secondary = secondary;
		}

		int IComparer<T>.Compare(T x, T y)
		{
			int result = primary.Compare(x, y);
	
			return result == 0 ? secondary.Compare(x, y) : result;
		}
	}
}