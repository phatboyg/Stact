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
	/// A channel that sends the most recent message to the consumer every interval
	/// </summary>
	/// <typeparam name="T">The type of message delivered on the channel</typeparam>
	public class LastIntervalChannel<T> :
		Channel<T>,
		IDisposable
	{
		private readonly Channel<T> _output;
		private readonly ActionQueue _queue;
		private bool _disposed;
		private T _lastMessage;
		private ScheduledAction _scheduledAction;

		/// <summary>
		/// Constructs a channel
		/// </summary>
		/// <param name="queue">The queue where consumer actions should be enqueued</param>
		/// <param name="scheduler">The scheduler to use for scheduling calls to the consumer</param>
		/// <param name="interval">The interval between calls to the consumer</param>
		/// <param name="output">The method to call when a message is sent to the channel</param>
		public LastIntervalChannel(ActionQueue queue, ActionScheduler scheduler, TimeSpan interval, Channel<T> output)
		{
			_queue = queue;
			_output = output;

			_scheduledAction = scheduler.Schedule(interval, interval, queue, SendMessageToOutputChannel);
		}

		public void Send(T message)
		{
			_queue.Enqueue(() => _lastMessage = message);
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

		private void SendMessageToOutputChannel()
		{
			_output.Send(_lastMessage);
		}

		~LastIntervalChannel()
		{
			Dispose(false);
		}
	}
}