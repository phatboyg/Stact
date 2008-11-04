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
namespace Magnum.Metrics
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Common;
	using Common.DateTimeExtensions;
	using Common.ObjectExtensions;

	public class IisLogReader :
		IEnumerable<IisLogEntry>
	{
		private static Dictionary<string, Action<Mapper<string[], IisLogEntry>, int>> _fieldMaps;
		private readonly ILineReader _reader;
		private Mapper<string[], IisLogEntry> _map;
		private string[] _mappedFields = new string[] {};

		static IisLogReader()
		{
			SetupFieldMap();
		}

		public IisLogReader(ILineReader reader, string[] mappedFields)
		{
			_reader = reader;

			RebuildMap(mappedFields);
		}

		public IisLogReader(ILineReader reader)
		{
			_reader = reader;

			CreateDefaultMap();
		}

		public IEnumerator<IisLogEntry> GetEnumerator()
		{
			foreach (string line in _reader)
			{
				if (line.IsNullOrEmpty())
					continue;

				if (line[0] == '#')
				{
					ProcessCommandLine(line);
					continue;
				}

				yield return _map.Transform(line.Split(' '));
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void ProcessCommandLine(string line)
		{
			if (line.Length > 8 && line.Substring(0, 8) == "#Fields:")
			{
				RebuildMap(line.Substring(9).Split(' '));
			}
		}

		private void CreateDefaultMap()
		{
			_map = new Mapper<string[], IisLogEntry>();
			_map.From(x => DateTime.Parse(string.Join(" ", x, 0, 2)).ForceUtc()).To(y => y.Date);
		}

		private void RebuildMap(string[] fields)
		{
			CreateDefaultMap();

			_mappedFields = fields;

			for (int index = 2; index < fields.Length; index++)
			{
				Action<Mapper<string[], IisLogEntry>, int> fieldMap;
				if (_fieldMaps.TryGetValue(fields[index], out fieldMap) == false)
					throw new ArgumentException("Unexpected log column: " + fields[index]);

				fieldMap(_map, index);
			}
		}

		private static void SetupFieldMap()
		{
			Func<string, string> nullIfDash = (x) => x == "-" ? null : x;

			_fieldMaps = new Dictionary<string, Action<Mapper<string[], IisLogEntry>, int>>
				{
					{"s-sitename", (m, i) => m.From(x => x[i]).To(y => y.SiteName)},
					{"s-computername", (m, i) => m.From(x => x[i]).To(y => y.ComputerName)},
					{"s-ip", (m, i) => m.From(x => x[i]).To(y => y.ServerIpAddress)},
					{"cs-method", (m, i) => m.From(x => x[i]).To(y => y.Method)},
					{"cs-uri-stem", (m, i) => m.From(x => x[i]).To(y => y.UriStem)},
					{"cs-uri-query", (m, i) => m.From(x => nullIfDash(x[i])).To(y => y.UriQuery)},
					{"s-port", (m, i) => m.From(x => int.Parse(x[i])).To(y => y.Port)},
					{"cs-username", (m, i) => m.From(x => nullIfDash(x[i])).To(y => y.Username)},
					{"c-ip", (m, i) => m.From(x => x[i]).To(y => y.RemoteIpAddress)},
					{"cs-version", (m, i) => m.From(x => x[i]).To(y => y.ProtocolVersion)},
					{"cs(User-Agent)", (m, i) => m.From(x => x[i]).To(y => y.UserAgent)},
					{"cs(Cookie)", (m, i) => m.From(x => nullIfDash(x[i])).To(y => y.Cookie)},
					{"cs(Referer)", (m, i) => m.From(x => nullIfDash(x[i])).To(y => y.Referer)},
					{"cs-host", (m, i) => m.From(x => nullIfDash(x[i])).To(y => y.Host)},
					{"sc-status", (m, i) => m.From(x => int.Parse(x[i])).To(y => y.Status)},
					{"sc-substatus", (m, i) => m.From(x => int.Parse(x[i])).To(y => y.SubStatus)},
					{"sc-win32-status", (m, i) => m.From(x => int.Parse(x[i])).To(y => y.Win32Status)},
					{"sc-bytes", (m, i) => m.From(x => int.Parse(x[i])).To(y => y.BytesSent)},
					{"cs-bytes", (m, i) => m.From(x => int.Parse(x[i])).To(y => y.BytesReceived)},
					{"time-taken", (m, i) => m.From(x => int.Parse(x[i])).To(y => y.TimeTaken)},
					{"", (m, i) => { }}
				};
		}
	}
}