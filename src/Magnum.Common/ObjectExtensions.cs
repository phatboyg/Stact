namespace Magnum.Common
{
	using System;

	public static class ObjectExtensions
	{
		public static void MustNotBeNull<T>(this T reference) where T : class
		{
			if (reference == null)
				throw new ArgumentNullException();
		}

		public static void MustNotBeNull<T>(this T reference, string name) where T : class
		{
			if (reference == null)
				throw new ArgumentNullException(name);
		}

		public static void MustNotBeNull<T>(this T reference, string name, string message) where T : class
		{
			if (reference == null)
				throw new ArgumentNullException(name, message);
		}

		public static void MustNotBeEmpty(this string value)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException();
		}

		public static void MustNotBeEmpty(this string value, string name)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("The argument must not be empty", name);
		}

		public static void MustNotBeEmpty(this string value, string name, string message)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException(message, name);
		}

		public static void MustBeInRange<T>(this T value, RangeBuilder<T> rangeBuilder)
		{
			Range<T> range = rangeBuilder;

			value.MustBeInRange(range);
		}

		public static void MustBeInRange<T>(this T value, Range<T> range)
		{
			if (!range.Contains(value))
				throw new ArgumentException();
		}
	}
}