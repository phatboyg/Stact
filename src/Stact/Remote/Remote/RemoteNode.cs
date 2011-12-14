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
    using Magnum.Extensions;
    using Magnum.Serialization;


    public class RemoteNode :
        Node
    {
        readonly BufferedChunkWriter _buffer;
        readonly Fiber _fiber;
        readonly Channel<ArraySegment<byte>> _reader;
        readonly Scheduler _scheduler;
        readonly HeaderChannel _writer;
        IList<IDisposable> _disposables;
        bool _disposed;

        public RemoteNode(UntypedChannel input, ChunkWriter output, FiberFactory fiberFactory, Scheduler scheduler,
                          Serializer serializer)
        {
            _disposables = new List<IDisposable>();

            _scheduler = scheduler;

            _fiber = fiberFactory();

            _buffer = new BufferedChunkWriter(_fiber, _scheduler, output, 64*1024);
            _buffer.Start();

            _reader = new DeserializeChunkChannel(input, serializer);
            _writer = new SerializeChunkChannel(_buffer, serializer);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Send(ArraySegment<byte> message)
        {
            _reader.Send(message);
        }

        public void Send<T>(Message<T> message, IDictionary<string, string> headers)
        {
            _writer.Send(message, headers);
        }

        public void Send<T>(Message<T> message)
        {
            _writer.Send(message);
        }

        ~RemoteNode()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _disposables.Each(x => x.Dispose());

                _buffer.Dispose();
                _fiber.Stop(1.Minutes());
            }

            _disposed = true;
        }

        public void AddDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }
    }
}