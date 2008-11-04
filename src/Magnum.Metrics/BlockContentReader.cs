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
	using System.Text;

	public class BlockContentReader : IContentReader
	{
		private static readonly string[] _separator = new[] {Environment.NewLine};
		private readonly IContentCollector _collector;
		private int _blockCount;
		private int _blockLength;
		private int _offset;

		public BlockContentReader(IContentCollector collector)
		{
			_collector = collector;
			_offset = 0;
			_blockLength = 200000;
			_blockCount = 25;
		}

		public IEnumerator<string> GetEnumerator()
		{
			for (int blockNumber = 0; blockNumber < _blockCount; blockNumber++)
			{
				string content = ReadBlock();
				if (content.Length == 0)
					yield break;

				string[] lines = content.Split(_separator, 1000, StringSplitOptions.None);
				foreach (string line in lines)
				{
					if (line.Length == 0)
					{
						_offset += Environment.NewLine.Length;
						continue;
					}

					_offset += line.Length + Environment.NewLine.Length;
					yield return line;
				}
			}
		}

		private string ReadBlock()
		{
			ArraySegment<byte> block = _collector.GetContentSegment(_offset, _blockLength);

			string content = Encoding.UTF8.GetString(block.Array, block.Offset, block.Count);

			int position = content.LastIndexOf(Environment.NewLine);
			if (position >= 0)
				content = content.Substring(0, position);
			return content;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}