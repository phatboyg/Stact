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
	using Magnum.Serialization;
	using ReliableMulticast;


	public class RemoteNode :
		HeaderChannel,
		IDisposable
	{
		readonly Fiber _fiber;
		readonly Uri _remoteAddress;
		readonly Scheduler _scheduler;
		readonly Serializer _serializer;
		readonly ReliableMulticastWriter _writer;

		BufferedChunkWriter _buffer;
		HeaderChannel _channel;
		bool _disposed;

		public RemoteNode(Fiber fiber, Scheduler scheduler, Serializer serializer, Uri remoteAddress)
		{
			_fiber = fiber;
			_scheduler = scheduler;
			_serializer = serializer;
			_remoteAddress = remoteAddress;
			_writer = new ReliableMulticastWriter(_remoteAddress);
		}

		public void Send<T>(T message, IDictionary<string, string> headers)
		{
			_channel.Send(message, headers);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start()
		{
			_writer.Start();

			var intercepter = new DelegateChunkWriter(chunk =>
				{
					Console.WriteLine(chunk.ToMemoryViewString());

					_writer.Write(chunk, x =>
						{
						});
				});

			_buffer = new BufferedChunkWriter(_fiber, _scheduler, intercepter, 64*1024);
			_buffer.Start();

			_channel = new ChunkWriterChannel(_buffer, _serializer);
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
				_buffer.Dispose();
				_writer.Dispose();
			}

			_disposed = true;
		}
	}
}