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
	using Extensions;
	using Fibers;

	public class SynchronousInbox<T> :
		Inbox<T>
	{
		private readonly Fiber _fiber;
		private readonly IList<SelectiveConsumer<T>> _receivers;
		private readonly Scheduler _scheduler;
		private readonly IList<T> _waitingMessages;

		public SynchronousInbox(Fiber fiber, Scheduler scheduler)
		{
			_fiber = fiber;
			_scheduler = scheduler;

			_receivers = new List<SelectiveConsumer<T>>();
			_waitingMessages = new List<T>();
		}

		public void Send(T message)
		{
			_fiber.Add(() => HandleSend(message));
		}

		public void Receive(SelectiveConsumer<T> consumer)
		{
			if (ReceiveWaitingMessage(consumer))
				return;

			_receivers.Add(consumer);
		}

		public void Receive(SelectiveConsumer<T> consumer, TimeSpan timeout, Action timeoutCallback)
		{
			if (ReceiveWaitingMessage(consumer))
				return;

			ScheduledAction scheduledAction = null;
			SelectiveConsumer<T> consumerProxy = msg =>
				{
					Consumer<T> c = consumer(msg);
					if (c == null)
						return null;

					return m =>
						{
							scheduledAction.Cancel();

							c(m);
						};
				};

			scheduledAction = _scheduler.Schedule(timeout, _fiber, () =>
				{
					_receivers.Remove(consumerProxy);

					timeoutCallback();
				});

			_receivers.Add(consumerProxy);
		}

		public void Receive(SelectiveConsumer<T> consumer, int timeout, Action timeoutCallback)
		{
			Receive(consumer, timeout.Milliseconds(), timeoutCallback);
		}

		private void HandleSend(T message)
		{
			if (DeliverToWaitingReceiver(message))
				return;

			_waitingMessages.Add(message);
		}

		private bool ReceiveWaitingMessage(SelectiveConsumer<T> selectiveConsumer)
		{
			for (int i = 0; i < _waitingMessages.Count; i++)
			{
				Consumer<T> consumer = selectiveConsumer(_waitingMessages[i]);
				if (consumer == null)
					continue;

				consumer(_waitingMessages[i]);

				_waitingMessages.RemoveAt(i);
				return true;
			}

			return false;
		}

		private bool DeliverToWaitingReceiver(T message)
		{
			for (int i = 0; i < _receivers.Count; i++)
			{
				SelectiveConsumer<T> receiver = _receivers[i];

				Consumer<T> consumer = receiver(message);
				if (consumer == null)
					continue;

				try
				{
					consumer(message);
				}
				finally
				{
					_receivers.RemoveAt(i);
				}
				return true;
			}

			return false;
		}
	}
}