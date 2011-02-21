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
	using System.Collections.Generic;
	using System.IO;
	using Magnum.Reflection;
	using Magnum.Serialization;
	using MessageHeaders;


	public class ChunkChannel :
		Channel<ArraySegment<byte>>
	{
		readonly UntypedChannel _output;
		readonly Serializer _serializer;

		public ChunkChannel(UntypedChannel output, Serializer serializer)
		{
			_output = output;
			_serializer = serializer;
		}

		public void Send(ArraySegment<byte> chunk)
		{
			int offset = 0;
			while (offset < chunk.Count)
			{
				int headerLength = BitConverter.ToInt32(chunk.Array, offset);
				int bodyLength = BitConverter.ToInt32(chunk.Array, offset + 4);

				if (offset + headerLength + bodyLength > chunk.Count)
				{
					// TODO we exceeded the buffer, this is bad
					return;
				}

				IDictionary<string, string> headers = DeserializeHeaders(chunk, offset, headerLength);

				DeserializeBody(chunk, offset + headerLength + 8, bodyLength, headers);

				offset = offset + 8 + headerLength + bodyLength;
			}
		}

		void DeserializeBody(ArraySegment<byte> chunk, int bodyOffset, int bodyLength, IDictionary<string, string> headers)
		{
			try
			{
				string typeName;
				if (!headers.TryGetValue("BodyType", out typeName))
				{
					// TODO we are discarding an invalid message type
				}

				var urn = new MessageUrn(typeName);
				Type messageType = urn.GetType();
				if (messageType == null)
				{
					// TODO log invalid/unknown message type
					return;
				}

				using (var bodyStream = new MemoryStream(chunk.Array, bodyOffset, bodyLength, false))
				using (TextReader bodyReader = new StreamReader(bodyStream))
					this.FastInvoke(new[] {messageType}, "Deserialize", bodyReader, headers);
			}
			catch (Exception ex)
			{
				// TODO bad msg
			}
		}

		IDictionary<string, string> DeserializeHeaders(ArraySegment<byte> chunk, int offset, int length)
		{
			IDictionary<string, string> headers;
			if (length > 0)
			{
				using (var headerStream = new MemoryStream(chunk.Array, offset + 8, length, false))
				using (var headerReader = new StreamReader(headerStream))
					headers = _serializer.Deserialize<IDictionary<string, string>>(headerReader);
			}
			else
				headers = new Dictionary<string, string>();

			return headers;
		}

		void Deserialize<TMessage>(TextReader reader, IDictionary<string, string> headers)
		{
			var message = _serializer.Deserialize<TMessage>(reader);

			SendToOutput(message, headers);
		}

		void SendToOutput<TBody>(TBody body, IDictionary<string, string> headers)
		{
			string method = headers[MessageMethod.HeaderKey];
			switch (method)
			{
				case MessageMethod.Request:
					_output.Send<Request<TBody>>(new RequestImpl<TBody>(new ShuntChannel(), body, headers));
					break;

				case MessageMethod.Response:
					_output.Send<Response<TBody>>(new ResponseImpl<TBody>(body, headers));
					break;

				default:
					_output.Send<Message<TBody>>(new MessageImpl<TBody>(body, headers));
					break;
			}
		}
	}
}