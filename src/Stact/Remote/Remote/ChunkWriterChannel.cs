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
	using Magnum;
	using Magnum.Serialization;


	public class ChunkWriterChannel :
		MessageChannel
	{
		const int PaddingLength = 8;
		readonly ChunkWriter _chunkWriter;
		readonly byte[] _padding;
		readonly Serializer _serializer;
		readonly Action<ArraySegment<byte>> _unsentCallback;
		int _messageBufferCapacity;

		public ChunkWriterChannel(ChunkWriter chunkWriter, Serializer serializer)
		{
			_padding = new byte[PaddingLength];
			_messageBufferCapacity = 4096;
			_chunkWriter = chunkWriter;
			_serializer = serializer;
			_unsentCallback = IgnoreUnsentCallback;
		}

		public int MessageBufferCapacity
		{
			get { return _messageBufferCapacity; }
			set
			{
				Guard.GreaterThan(0, value);

				_messageBufferCapacity = value;
			}
		}

		public void Send<T>(T message)
		{
			Send(message, new Dictionary<string, string>());
		}

		public void Send<T>(T message, IDictionary<string, string> headers)
		{
			using (var ms = new MemoryStream(_messageBufferCapacity))
			using (var ts = new StreamWriter(ms))
			{
				ms.Write(_padding, 0, PaddingLength);

				headers["BodyType"] = typeof(T).ToMessageUrn().ToString();

				int headerLength = 0;
				if (headers.Count > 0)
				{
					_serializer.Serialize(headers, ts);
					ts.Flush();
					headerLength = (int)ms.Length - PaddingLength;
				}

				_serializer.Serialize(message, ts);
				ts.Flush();

				int bodyLength = (int)ms.Length - headerLength - PaddingLength;

				byte[] buffer = ms.GetBuffer();

				byte[] bytes = BitConverter.GetBytes(headerLength);
				Array.Copy(bytes, 0, buffer, 0, 4);

				bytes = BitConverter.GetBytes(bodyLength);
				Array.Copy(bytes, 0, buffer, 4, 4);

				var arraySegment = new ArraySegment<byte>(buffer, 0, (int)ms.Length);

				_chunkWriter.Write(arraySegment, _unsentCallback);
			}
		}

		static void IgnoreUnsentCallback(ArraySegment<byte> obj)
		{
		}
	}
}