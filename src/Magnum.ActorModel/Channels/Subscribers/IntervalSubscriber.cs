namespace Magnum.ActorModel.Channels.Subscribers
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