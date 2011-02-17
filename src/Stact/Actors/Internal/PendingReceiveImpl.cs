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
namespace Stact.Internal
{
	using System;
	using Configuration;


	public class PendingReceiveImpl<TMessage> :
		PendingReceive
	{
		readonly Action<PendingReceiveImpl<TMessage>> _onComplete;
		readonly SelectiveConsumer<TMessage> _selectiveConsumer;
		readonly Action _timeoutCallback;
		bool _cancel;
		Inbox _inbox;
		ScheduledOperation _scheduledAction;

		public PendingReceiveImpl(Inbox inbox, SelectiveConsumer<TMessage> selectiveConsumer, Action timeoutCallback,
		                          Action<PendingReceiveImpl<TMessage>> onComplete)
		{
			_selectiveConsumer = selectiveConsumer;
			_inbox = inbox;
			_timeoutCallback = timeoutCallback;
			_onComplete = onComplete;
		}

		public PendingReceiveImpl(Inbox inbox, SelectiveConsumer<TMessage> selectiveConsumer,
		                          Action<PendingReceiveImpl<TMessage>> onComplete)
			: this(inbox, selectiveConsumer, NoTimeoutCallback, onComplete)
		{
		}

		public void Cancel()
		{
			_cancel = true;

			_onComplete(this);
		}

		public void Send<T>(T message)
		{
			_inbox.Send(message);
		}

		public ChannelConnection Connect(Action<ConnectionConfigurator> subscriberActions)
		{
			return _inbox.Connect(subscriberActions);
		}

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer)
		{
			return _inbox.Receive(consumer);
		}

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer, TimeSpan timeout, Action timeoutCallback)
		{
			return _inbox.Receive(consumer, timeout, timeoutCallback);
		}

		public void ScheduleTimeout(Func<PendingReceiveImpl<TMessage>, ScheduledOperation> scheduleAction)
		{
			_scheduledAction = scheduleAction(this);
		}

		static void NoTimeoutCallback()
		{
		}

		public Consumer<TMessage> Accept(TMessage message)
		{
			if (_cancel)
				return null;

			Consumer<TMessage> consumer = _selectiveConsumer(message);
			if (consumer == null)
				return null;

			return m =>
				{
					CancelTimeout();

					consumer(m);

					_onComplete(this);
				};
		}

		public void Timeout()
		{
			if (_cancel)
				return;

			_timeoutCallback();

			_onComplete(this);
		}

		void CancelTimeout()
		{
			if (_scheduledAction != null)
				_scheduledAction.Cancel();
		}
	}
}