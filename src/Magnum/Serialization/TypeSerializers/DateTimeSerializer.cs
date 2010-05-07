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
namespace Magnum.Serialization.TypeSerializers
{
	using System;
	using System.Globalization;
	using System.Xml;
	using Extensions;

	public class DateTimeSerializer :
		TypeSerializer<DateTime>
	{
		public const string DateFormat = "yyyy-MM-dd";
		public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
		public const string DateTimeMillisecondsFormat = "yyyy-MM-ddTHH:mm:ss.fffffffZ";
		public const string DateTimeShortMillisecondsFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

		public TypeReader<DateTime> GetReader()
		{
			return ParseShortestXsdDateTime;
		}

		public TypeWriter<DateTime> GetWriter()
		{
			return (value, output) => output(GetShortestDateTimeString(value));
		}

		private static string GetShortestDateTimeString(DateTime dateTime)
		{
			TimeSpan timeOfDay = dateTime.TimeOfDay;
			if (timeOfDay.Ticks == 0)
				return dateTime.ToString(DateFormat);

			if (timeOfDay.Milliseconds == 0)
				return dateTime.ToUniversalTime().ToString(DateTimeFormat);

			return XmlConvert.ToString(dateTime.ToUniversalTime(), XmlDateTimeSerializationMode.Utc);
		}

		private static DateTime ParseShortestXsdDateTime(string text)
		{
			if (text.IsEmpty())
				return DateTime.MinValue;

			if (text.Length <= DateTimeMillisecondsFormat.Length || text.Length >= DateTimeShortMillisecondsFormat.Length)
				return XmlConvert.ToDateTime(text, XmlDateTimeSerializationMode.Utc);

			if (text.Length == DateTimeFormat.Length)
				return DateTime.ParseExact(text, DateTimeFormat, null, DateTimeStyles.AdjustToUniversal);

			return new DateTime(
				int.Parse(text.Substring(0, 4)),
				int.Parse(text.Substring(5, 2)),
				int.Parse(text.Substring(8, 2)),
				0, 0, 0,
				DateTimeKind.Utc);
		}
	}
}