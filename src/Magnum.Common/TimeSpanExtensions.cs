namespace Magnum.Common
{
	using System;

	public static class TimeSpanExtensions
	{
		/// <summary>
		/// Creates a TimeSpan for the specified number of weeks
		/// </summary>
		/// <param name="value">The number of weeks</param>
		/// <returns></returns>
		public static TimeSpan Weeks(this int value)
		{
			return TimeSpan.FromDays(value * 7);
		}

		/// <summary>
		/// Creates a TimeSpan for the specified number of days
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TimeSpan Days(this int value)
		{
			return TimeSpan.FromDays(value);
		}

		/// <summary>
		/// Creates a TimeSpan for the specified number of hours
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TimeSpan Hours(this int value)
		{
			return TimeSpan.FromHours(value);
		}

		/// <summary>
		/// Creates a TimeSpan for the specified number of minutes
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TimeSpan Minutes(this int value)
		{
			return TimeSpan.FromMinutes(value);
		}

		/// <summary>
		/// Creates a TimeSpan for the specified number of seconds
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TimeSpan Seconds(this int value)
		{
			return TimeSpan.FromSeconds(value);
		}

		/// <summary>
		/// Creates a TimeSpan for the specified number of milliseconds
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TimeSpan Milliseconds(this int value)
		{
			return TimeSpan.FromMilliseconds(value);
		}
	}
}