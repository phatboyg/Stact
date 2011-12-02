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
    using System.Diagnostics;
    using System.IO;
    using Magnum.Reflection;
    using Magnum.Serialization;
    using MessageHeaders;


    public class DeserializeChunkChannel :
        Channel<ArraySegment<byte>>
    {
        readonly UntypedChannel _output;
        readonly Serializer _serializer;
        readonly TraceSource _ts;

        public DeserializeChunkChannel(UntypedChannel output, Serializer serializer)
        {
            _ts = new TraceSource("Stact.Remote", SourceLevels.All);

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
                    _ts.TraceEvent(TraceEventType.Error, RemoteError.BadRequest, "Invalid header or body length specified");
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
                    _ts.TraceEvent(TraceEventType.Error, RemoteError.BadRequest, "BodyType not specified");
                    return;
                }

                var urn = new MessageUrn(typeName);
                Type messageType = urn.GetType();
                if (messageType == null)
                {
                    _ts.TraceEvent(TraceEventType.Error, RemoteError.BadRequest, "BodyType not recognized, discarded");
                    return;
                }

                using (var bodyStream = new MemoryStream(chunk.Array, bodyOffset, bodyLength, false))
                using (TextReader bodyReader = new StreamReader(bodyStream))
                    this.FastInvoke(new[] {messageType}, "Deserialize", bodyReader, headers);
            }
            catch (Exception)
            {
                _ts.TraceEvent(TraceEventType.Error, RemoteError.BadRequest, "Unexpected error deserializing message");
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

// ReSharper disable UnusedMember.Local
        void Deserialize<TMessage>(TextReader reader, IDictionary<string, string> headers)
// ReSharper restore UnusedMember.Local
        {
            var message = _serializer.Deserialize<TMessage>(reader);

            SendToOutput(message, headers);
        }

        void SendToOutput<TBody>(TBody body, IDictionary<string, string> headers)
        {
            _output.Send<Message<TBody>>(new MessageContext<TBody>(body, headers));
        }
    }
}