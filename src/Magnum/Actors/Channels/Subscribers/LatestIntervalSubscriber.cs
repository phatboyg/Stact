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

	public class LatestIntervalSubscriber<T> :
		SubscriberBase<T>
	{
		private readonly Action<T> _consume;
		private readonly int _interval;
		private readonly object _lock = new object();
		private readonly Scheduler _scheduler;
		private bool _flushScheduled;
		private T _pending;

		public LatestIntervalSubscriber(Action<T> consume, int interval, Scheduler scheduler)
		{
			_consume = consume;
			_interval = interval;
			_scheduler = scheduler;
			_flushScheduled = false;
		}

		protected override void ConsumeMessage(T message)
		{
			lock (_lock)
			{
				if (!_flushScheduled)
				{
					_scheduler.Schedule(_interval, Flush);
					_flushScheduled = true;
				}

				_pending = message;
			}
		}

		private void Flush()
		{
			T pending;

			lock (_lock)
			{
				pending = _pending;

				_flushScheduled = false;
			}

			_consume(pending);
		}
	}
}