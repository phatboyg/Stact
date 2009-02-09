namespace Magnum.ActorModel.Channels
{
	using System;
	using System.Collections.Generic;

	public class UniqueIntervalSubscriber<K,T> :
		SubscriptionBase<T>
	{
		private readonly Action<IDictionary<K,T>> _consume;
		private readonly int _interval;
		private readonly Converter<T, K> _converter;
		private readonly IActionScheduler _scheduler;
		private readonly object _lock = new object();
		private Dictionary<K,T> _pending;

		public UniqueIntervalSubscriber(Action<IDictionary<K,T>> consume, int interval, Converter<T,K> converter, IActionScheduler scheduler)
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
			IDictionary<K,T> messages;

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