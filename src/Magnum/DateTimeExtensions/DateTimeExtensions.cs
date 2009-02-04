// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Common.DateTimeExtensions
{
	using System;

	public static class DateTimeExt
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

        public static DateTime ForceUtc(this DateTime value)
        {
            if (value.Kind == DateTimeKind.Utc)
                return value;

            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, DateTimeKind.Utc);
        }
	}
}