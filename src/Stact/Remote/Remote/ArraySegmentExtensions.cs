// Copyright 2010 Chris Patterson
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
namespace Stact.Remote
{
	using System;
	using System.Text;


	public static class ArraySegmentExtensions
	{
		public static string ToMemoryViewString(this ArraySegment<byte> segment)
		{
			var sb = new StringBuilder();

			for (int i = 0; i < segment.Count;)
			{
				string printable = "  ";

				int j = 0;
				for (; j < 16 && i < segment.Count; j++, i++)
				{
					byte b = segment.Array[i];

					sb.Append(b.ToString("X2"));
					sb.Append(' ');
					if (b >= 0x20 && b <= 0x7F)
						printable = printable + Convert.ToChar(b);
					else
						printable = printable + ".";
				}
				for (; j < 16; j++)
					sb.Append("   ");

				sb.AppendLine(printable);
			}

			return sb.ToString();
		}
	}
}