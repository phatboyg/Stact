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
namespace Magnum.Channels
{
	using System;
	using Actions;

	/// <summary>
	/// A channel that accepts a message and enqueues the consumer method via the
	/// specified ActionQueue
	/// </summary>
	/// <typeparam name="T">The type of message delivered on the channel</typeparam>
	public class ConsumerChannel<T> :
		Channel<T>
	{
		private readonly Consumer<T> _consumer;
		private readonly ActionQueue _queue;
		private bool _disposed;

		/// <summary>
		/// Constructs a channel
		/// </summary>
		/// <param name="queue">The queue where consumer actions should be enqueued</param>
		/// <param name="consumer">The method to call when a message is sent to the channel</param>
		public ConsumerChannel(ActionQueue queue, Consumer<T> consumer)
		{
			_queue = queue;
			_consumer = consumer;
		}

		public void Send(T message)
		{
			_queue.Enqueue(() => _consumer(message));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
			}

			_disposed = true;
		}

		~ConsumerChannel()
		{
			Dispose(false);
		}
	}
}