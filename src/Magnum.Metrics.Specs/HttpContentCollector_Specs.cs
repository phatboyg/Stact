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
namespace Magnum.Metrics.Specs
{
	using System;
	using System.Collections.Specialized;
	using System.Diagnostics;
	using System.Web;
	using System.Linq;
	using Common.DateTimeExtensions;
	using Common.ObjectExtensions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_pulling_information_about_the_remote_file
	{
		private string _filename = "ex" + DateTime.Now.ToString("yyMMdd") + ".log";
		private const string _baseUrl = "http://192.168.105.125/Web4Logs/";

		[Test]
		public void The_linq_language_should_be_supported()
		{
			IContentCollector collector = new HttpContentCollector(_baseUrl + _filename);
			IContentReader reader = new BlockContentReader(collector);
			WebServerLogReader logReader = new WebServerLogReader(reader);

			var matches = logReader.Where(x => x.UriStem.Contains("CFR.asp")).Count();

			Trace.WriteLine("Matches = " + matches);
		}

		[Test]
		public void A_remote_log_file_should_be_loaded_into_the_IIS_reader()
		{
			IContentCollector collector = new HttpContentCollector(_baseUrl + _filename);
			IContentReader reader = new BlockContentReader(collector);
			WebServerLogReader logReader = new WebServerLogReader(reader);

			int count = 0;
			foreach (WebServerLogEntry line in logReader)
			{
				count++;

//				CookieDictionary cookies = line.Cookies;
//				foreach (KeyValuePair<string, string> pair in cookies)
//				{
//					Trace.WriteLine(string.Format("Cookie ({0}) = {1}", pair.Key, pair.Value));
//				}

				string queryString = line.UriQuery;
				if (!queryString.IsNullOrEmpty())
				{
					NameValueCollection values = HttpUtility.ParseQueryString(queryString);
					foreach (string value in values)
					{
						Trace.WriteLine(value + ": " + values[value]);
					}
				}
			}

			foreach (WebServerLogEntry line in logReader)
			{
				count++;
			}

			Trace.WriteLine("Lines Read: " + count);

			Assert.That(count, Is.GreaterThan(0));
		}

		[Test]
		public void A_segment_of_the_log_should_be_returned()
		{
			HttpContentCollector collector = new HttpContentCollector(_baseUrl + _filename);

			ArraySegment<byte> segment = collector.GetContentSegment(0, 10000);

			Assert.That(segment.Count, Is.LessThanOrEqualTo(10000));
		}

		[Test]
		public void The_file_should_have_been_modified_recently()
		{
			HttpContentCollector collector = new HttpContentCollector(_baseUrl + _filename);

			DateTime lastModified = collector.GetLastModified();

			Trace.WriteLine("Last Modified: " + lastModified);

			Assert.That(lastModified, Is.GreaterThanOrEqualTo(DateTime.Now.Midnight()));
		}

		[Test]
		public void The_length_of_the_content_should_be_more_than_zero()
		{
			HttpContentCollector collector = new HttpContentCollector(_baseUrl + _filename);

			long length = collector.GetContentLength();

			Trace.WriteLine("Length: " + length);

			Assert.That(length, Is.GreaterThan(0));
		}
	}
}