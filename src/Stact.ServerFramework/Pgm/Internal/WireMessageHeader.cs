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
namespace Stact.ServerFramework.Internal
{
	using System;
	using System.IO;





	public struct WireMessagePropertyHeader
	{
		public static Int16 SourceAddress = -1;
		public static Int16 DestinationAddress = -2;
		public static Int16 ResponseAddress = -3;
		public static Int16 FaultAddress = -4;
		public static Int16 MessageType = -5;
		public static Int16 BodyFormat = -6;
		public static Int16 MessageId = -7;
		public static Int16 ConversationId = -8;
		public static Int16 ResponseId = -9;

		readonly Int16 _keyLength;
		readonly Int16 _valueLength;

		public void GetHeader()
		{
			byte[] buffer = new byte[4096];

			ArraySegment<byte> block = new ArraySegment<byte>(buffer, 0, 4);

			BitConverter.ToInt32(block.Array, block.Offset);
		}

	}


	public struct WireMessageHeader
	{
		readonly int _headerLength;
		readonly int _bodyLength;

		public int HeaderLength
		{
			get { return _headerLength; }
		}

		public int BodyLength
		{
			get { return _bodyLength; }
		}

		public int Length
		{
			get { return _headerLength + _bodyLength; }
		}

		public WireMessageHeader(int headerLength, int bodyLength)
		{
			_headerLength = headerLength;
			_bodyLength = bodyLength;
		}

		public void WriteToBuffer(byte[] buffer, int offset, int length)
		{
			var bytes = BitConverter.GetBytes(_headerLength);
			Array.Copy(bytes, 0, buffer, offset, 4);

			bytes = BitConverter.GetBytes(_bodyLength);
			Array.Copy(bytes, 0, buffer, offset + 4, 4);
		}

		public static WireMessageHeader CreateFromBuffer(byte[] buffer, int offset, int length)
		{
			int headerLength = BitConverter.ToInt32(buffer, offset);
			int bodyLength = BitConverter.ToInt32(buffer, offset + 4);

			return new WireMessageHeader(headerLength, bodyLength);
		}
	}
}