// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.Actors.Internal
{
	using System;
	using Channels;
	using Fibers;


	public class PendingReceiveImpl<T> :
		PendingReceive
	{
		readonly Action<PendingReceiveImpl<T>> _onComplete;
		readonly SelectiveConsumer<T> _selectiveConsumer;
		readonly Action _timeoutCallback;
		bool _cancel;
		ScheduledAction _scheduledAction;

		public PendingReceiveImpl(SelectiveConsumer<T> selectiveConsumer, Action timeoutCallback,
		                          Action<PendingReceiveImpl<T>> onComplete)
		{
			_selectiveConsumer = selectiveConsumer;
			_timeoutCallback = timeoutCallback;
			_onComplete = onComplete;
		}

		public PendingReceiveImpl(SelectiveConsumer<T> selectiveConsumer, Action<PendingReceiveImpl<T>> onComplete)
			: this(selectiveConsumer, NoTimeoutCallback, onComplete)
		{
		}

		public void Cancel()
		{
			_cancel = true;

			_onComplete(this);
		}

		public void ScheduleTimeout(Func<PendingReceiveImpl<T>, ScheduledAction> scheduleAction)
		{
			_scheduledAction = scheduleAction(this);
		}

		static void NoTimeoutCallback()
		{
		}

		public Consumer<T> Accept(T message)
		{
			if (_cancel)
				return null;

			Consumer<T> consumer = _selectiveConsumer(message);
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