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
namespace Stact.Extensions
{
	using System;
	using System.Collections.Generic;

	public static class ExtensionsToTimeSpan
	{
		/// <summary>
		/// Creates a TimeSpan for the specified number of weeks
		/// </summary>
		/// <param name="value">The number of weeks</param>
		/// <returns></returns>
		public static TimeSpan Weeks(this int value)
		{
			return TimeSpan.FromDays(value*7);
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
		public static TimeSpan Seconds(this double value)
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

		/// <summary>
		/// Returns an enumeration of the specified TimeSpan with the specified number of elements
		/// </summary>
		/// <param name="value">The TimeSpan to repeat</param>
		/// <param name="times">The number of times to repeat the TimeSpan</param>
		/// <returns>An enumeration of TimeSpan</returns>
		public static IEnumerable<TimeSpan> Repeat(this TimeSpan value, int times)
		{
			for (int i = 0; i < times; i++)
			{
				yield return value;
			}
		}
	}
}