namespace Magnum.ActorModel.Channels.Subscribers
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