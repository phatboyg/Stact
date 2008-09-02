namespace Magnum.Common
{
	using System;

	public static class DateTimeExtensions
	{
		public static DateTime FromNow(this TimeSpan span)
		{
			return DateTime.Now + span;
		}

		public static DateTime FromUtcNow(this TimeSpan span)
		{
			return DateTime.UtcNow + span;
		}

		public static DateTime First(this DateTime value)
		{
			return value.AddDays(1 - value.Day);
		}

		public static DateTime First(this DateTime value, DayOfWeek dayOfWeek)
		{
			DateTime first = value.First();

			if (first.DayOfWeek != dayOfWeek)
				first = first.Next(dayOfWeek);

			return first;
		}

		public static DateTime Last(this DateTime value)
		{
			int daysInMonth = DateTime.DaysInMonth(value.Year, value.Month);

			return value.AddDays(1 - value.Day + daysInMonth - 1);
		}

		public static DateTime Last(this DateTime value, DayOfWeek dayOfWeek)
		{
			DateTime last = value.Last();

			return last.AddDays(Math.Abs(dayOfWeek - last.DayOfWeek)*-1);
		}

		public static DateTime Next(this DateTime value, DayOfWeek dayOfWeek)
		{
			int offsetDays = dayOfWeek - value.DayOfWeek;

			if (offsetDays <= 0)
				offsetDays += 7;

			return value.AddDays(offsetDays);
		}

		public static DateTime Midnight(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.Day);
		}

		public static DateTime Noon(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.Day, 12, 0, 0);
		}

		public static DateTime SetTime(this DateTime value, int hour, int minute)
		{
			return SetTime(value, hour, minute, 0, 0);
		}

		public static DateTime SetTime(this DateTime value, int hour, int minute, int second)
		{
			return SetTime(value, hour, minute, second, 0);
		}

		public static DateTime SetTime(this DateTime value, int hour, int minute, int second, int millisecond)
		{
			return new DateTime(value.Year, value.Month, value.Day, hour, minute, second, millisecond);
		}
	}
}