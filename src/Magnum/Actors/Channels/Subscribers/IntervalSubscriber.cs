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
namespace Magnum.Actors.Channels.Subscribers
{
	using System;
	using System.Collections.Generic;

	public class IntervalSubscriber<T> :
		SubscriberBase<T>
	{
		private readonly Action<IList<T>> _consume;
		private readonly int _interval;
		private readonly object _lock = new object();
		private readonly Scheduler _scheduler;
		private List<T> _pending;

		public IntervalSubscriber(Action<IList<T>> consume, int interval, Scheduler scheduler)
		{
			_consume = consume;
			_interval = interval;
			_scheduler = scheduler;
		}

		protected override void ConsumeMessage(T message)
		{
			lock (_lock)
			{
				if (_pending == null)
				{
					_pending = new List<T>();
					_scheduler.Schedule(_interval, Flush);
				}
				_pending.Add(message);
			}
		}

		private void Flush()
		{
			IList<T> messages;

			lock (_lock)
			{
				if (_pending == null) return;

				messages = _pending;
				_pending = null;
			}

			_consume(messages);
		}
	}
}