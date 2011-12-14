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


    public class SerializeChunkChannel :
        HeaderChannel
    {
        const int PaddingLength = 8;
        static readonly IDictionary<string, string> _noHeaders = new Dictionary<string, string>();
        readonly ChunkWriter _chunkWriter;
        readonly byte[] _padding;
        readonly Serializer _serializer;
        int _bufferCapacity;
        Func<Stream, Stream> _contentEncoder;
        Action<ArraySegment<byte>> _unsentCallback;

        public SerializeChunkChannel(ChunkWriter chunkWriter, Serializer serializer)
        {
            _padding = new byte[PaddingLength];
            _bufferCapacity = 4096;
            _chunkWriter = chunkWriter;
            _serializer = serializer;
            _unsentCallback = DefaultUnsentCallback;
            _contentEncoder = DefaultContentEncoder;
        }

        public int BufferCapacity
        {
            get { return _bufferCapacity; }
            set
            {
                Guard.GreaterThan(0, value);

                _bufferCapacity = value;
            }
        }

        public Func<Stream, Stream> ContentEncoder
        {
            get { return _contentEncoder; }
            set { _contentEncoder = value ?? DefaultContentEncoder; }
        }

        public Action<ArraySegment<byte>> UnsentCallback
        {
            get { return _unsentCallback; }
            set { _unsentCallback = value ?? DefaultUnsentCallback; }
        }

        public void Send<T>(Message<T> message, IDictionary<string, string> headers)
        {
            using (var ms = new MemoryStream(_bufferCapacity))
            {
                ms.Write(_padding, 0, PaddingLength);

                using (var ts = new StreamWriter(_contentEncoder(ms)))
                {
                    headers[HeaderKey.BodyType] = MessageUrn<T>.UrnString;

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
        }

        public void Send<T>(Message<T> message)
        {
            Send(message, _noHeaders);
        }

        static Stream DefaultContentEncoder(Stream stream)
        {
            return stream;
        }

        static void DefaultUnsentCallback(ArraySegment<byte> obj)
        {
        }
    }
}