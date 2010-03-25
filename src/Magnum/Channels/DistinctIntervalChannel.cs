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
	using System.Collections.Generic;
	using Actions;
	using Internal;

	/// <summary>
	/// A channel that accepts distinct messages and sends them to the channel at regular intervals
	/// </summary>
	/// <typeparam name="T">The type of message delivered on the channel</typeparam>
	/// <typeparam name="TKey">The type of the key for the message</typeparam>
	public class DistinctIntervalChannel<T, TKey> :
		Channel<T>,
		IDisposable
	{
		private readonly IMessageDictionary<TKey, T> _messages;
		private readonly ActionQueue _queue;
		private readonly Channel<IDictionary<TKey, T>> _output;
		private bool _disposed;
		private ScheduledAction _scheduledAction;

		/// <summary>
		/// Constructs a channel
		/// </summary>
		/// <param name="queue">The queue where consumer actions should be enqueued</param>
		/// <param name="scheduler">The scheduler to use for scheduling calls to the consumer</param>
		/// <param name="interval">The interval between calls to the consumer</param>
		/// <param name="getKey">Returns the key for the message</param>
		/// <param name="output">The method to call when a message is sent to the channel</param>
		public DistinctIntervalChannel(ActionQueue queue, ActionScheduler scheduler, TimeSpan interval, Func<T, TKey> getKey, Channel<IDictionary<TKey, T>> output)
		{
			_messages = new MessageDictionary<TKey, T>(getKey);

			_queue = queue;
			_output = output;

			_scheduledAction = scheduler.Schedule(interval, interval, queue, SendMessagesToOutputChannel);
		}

		public void Send(T message)
		{
			_messages.Add(message);
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
				_scheduledAction.Cancel();
				_scheduledAction = null;
			}

			_disposed = true;
		}

		private void SendMessagesToOutputChannel()
		{
			_queue.Enqueue(() => _output.Send(_messages.RemoveAll()));
		}

		~DistinctIntervalChannel()
		{
			Dispose(false);
		}
	}
}