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
namespace Stact
{
	using System;
	using System.Collections.Generic;
	
	using Internal;


	/// <summary>
	/// A channel that accepts messages and sends them to the channel at regular intervals
	/// </summary>
	/// <typeparam name="T">The type of message delivered on the channel</typeparam>
	public class IntervalChannel<T> :
		Channel<T>,
		IDisposable
	{
		readonly Fiber _fiber;
		readonly MessageList<T> _messages;
		readonly Scheduler _scheduler;
		bool _disposed;
		ScheduledExecutionHandle _scheduledAction;

		/// <summary>
		/// Constructs a channel
		/// </summary>
		/// <param name="fiber">The queue where consumer actions should be enqueued</param>
		/// <param name="scheduler">The scheduler to use for scheduling calls to the consumer</param>
		/// <param name="interval">The interval between calls to the consumer</param>
		/// <param name="output">The method to call when a message is sent to the channel</param>
		public IntervalChannel(Fiber fiber, Scheduler scheduler, TimeSpan interval, Channel<ICollection<T>> output)
		{
			_messages = new MessageListImpl<T>();

			_fiber = fiber;
			_scheduler = scheduler;
			Output = output;
			Interval = interval;

			ScheduleNextSend();
		}

		public Channel<ICollection<T>> Output { get; private set; }

		public TimeSpan Interval { get; private set; }

		public void Send(T message)
		{
			_fiber.Execute(() => _messages.Add(message));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void ScheduleNextSend()
		{
			_scheduledAction = _scheduler.Schedule(Interval, _fiber, SendMessagesToOutputChannel);
		}

		void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			if (disposing)
			{
				_scheduledAction.Cancel();
				_scheduledAction = null;
			}

			_disposed = true;
		}

		void SendMessagesToOutputChannel()
		{
			IList<T> all = _messages.RemoveAll();
			if (all.Count > 0)
				Output.Send(all);

			ScheduleNextSend();
		}

		~IntervalChannel()
		{
			Dispose(false);
		}
	}
}