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
	using System.Collections.Generic;
	using System.IO;
	using Magnum.Reflection;
	using Magnum.Serialization;


	public class ChannelBlockReader :
		BlockReader
	{
		readonly UntypedChannel _output;
		readonly Serializer _serializer;

		public ChannelBlockReader(UntypedChannel output, Serializer serializer)
		{
			_output = output;
			_serializer = serializer;
		}

		public void Read(ArraySegment<byte> block)
		{
			int offset = 0;
			while (offset < block.Count)
			{
				int headerLength = BitConverter.ToInt32(block.Array, offset);
				int bodyLength = BitConverter.ToInt32(block.Array, offset + 4);

				if (offset + headerLength + bodyLength > block.Count)
				{
					// TODO we exceeded the buffer, this is bad
					return;
				}

				IDictionary<string, string> headers = DeserializeHeaders(block, offset, headerLength);

				DeserializeBody(block, offset + headerLength + 8, bodyLength, headers);

				offset = offset + 8 + headerLength + bodyLength;
			}
		}

		void DeserializeBody(ArraySegment<byte> block, int bodyOffset, int bodyLength, IDictionary<string, string> headers)
		{
			try
			{
				string typeName;
				if (!headers.TryGetValue("BodyType", out typeName))
				{
					// TODO we are discarding an invalid message type
				}

				Type messageType = MessageUrn.GetMessageType(typeName);
				if (messageType == null)
				{
					// TODO log invalid/unknown message type
					return;
				}

				using (var bodyStream = new MemoryStream(block.Array, bodyOffset, bodyLength, false))
				using (TextReader bodyReader = new StreamReader(bodyStream))
					this.FastInvoke(new[] {messageType}, "Deserialize", bodyReader);
			}
			catch (Exception ex)
			{
				// TODO bad msg
			}
		}

		IDictionary<string, string> DeserializeHeaders(ArraySegment<byte> block, int offset, int length)
		{
			IDictionary<string, string> headers;
			if (length > 0)
			{
				using (var headerStream = new MemoryStream(block.Array, offset + 8, length, false))
				using (var headerReader = new StreamReader(headerStream))
					headers = _serializer.Deserialize<IDictionary<string, string>>(headerReader);
			}
			else
				headers = new Dictionary<string, string>();

			return headers;
		}

		void Deserialize<TMessage>(TextReader reader)
		{
			var message = _serializer.Deserialize<TMessage>(reader);

			_output.Send(message);
		}
	}
}