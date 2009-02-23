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

	public class DistinctIntervalSubscriber<K, T> :
		SubscriberBase<T>
	{
		private readonly Action<IDictionary<K, T>> _consume;
		private readonly Converter<T, K> _converter;
		private readonly int _interval;
		private readonly object _lock = new object();
		private readonly Scheduler _scheduler;
		private Dictionary<K, T> _pending;

		public DistinctIntervalSubscriber(Action<IDictionary<K, T>> consume, int interval, Converter<T, K> converter, Scheduler scheduler)
		{
			_consume = consume;
			_interval = interval;
			_converter = converter;
			_scheduler = scheduler;
		}

		protected override void ConsumeMessage(T message)
		{
			lock (_lock)
			{
				if (_pending == null)
				{
					_pending = new Dictionary<K, T>();
					_scheduler.Schedule(_interval, Flush);
				}
				K key = _converter(message);
				_pending[key] = message;
			}
		}

		private void Flush()
		{
			IDictionary<K, T> messages;

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