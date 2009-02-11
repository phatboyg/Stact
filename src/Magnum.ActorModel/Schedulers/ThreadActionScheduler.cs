namespace Magnum.ActorModel.Schedulers
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;

	public class ThreadActionScheduler :
		Scheduler
	{
		private static readonly long _freq = Stopwatch.Frequency;
		private static readonly double MsMultiplier = 1000.00/_freq;

		private readonly object _lock = new object();
		private readonly SortedList<long, List<ScheduledEvent>> _pending = new SortedList<long, List<ScheduledEvent>>();
		private readonly long _startTimeInTicks = Stopwatch.GetTimestamp();

		private bool _enabled = true;
		private ManualResetEvent _waiter;

		private long Now
		{
			get { return (long) ((Stopwatch.GetTimestamp() - _startTimeInTicks)*MsMultiplier); }
		}

		public Unschedule Schedule(int interval, Action action)
		{
			var pending = new SingleEvent(interval, action, Now);

			QueueEvent(pending);
			return pending.Cancel;
		}

		public Unschedule Schedule(int initialInterval, int periodicInterval, Action action)
		{
			var pending = new RecurringEvent(initialInterval, periodicInterval, action, Now);

			QueueEvent(pending);
			return pending.Cancel;
		}

		public void Dispose()
		{
			_enabled = false;
		}


		public void QueueEvent(ScheduledEvent pending)
		{
			lock (_lock)
			{
				AddScheduledEvent(pending);
				if (_waiter != null)
				{
					_waiter.Set();
					_waiter = null;
				}
				else
				{
					WaitExpired(null, false);
				}
			}
		}

		private void AddScheduledEvent(ScheduledEvent pending)
		{
			List<ScheduledEvent> list;
			if (!_pending.TryGetValue(pending.ScheduledTime, out list))
			{
				list = new List<ScheduledEvent>(2);
				_pending[pending.ScheduledTime] = list;
			}
			list.Add(pending);
		}

		private void WaitExpired(object sender, bool timeout)
		{
			if (!_enabled) return;

			lock (_lock)
			{
				do
				{
					var rescheduled = ExecuteExpired();
					Queue(rescheduled);
				} while (!ScheduleTimerCallback());
			}
		}

		private bool ScheduleTimerCallback()
		{
			if (_pending.Count <= 0)
				return true;

			long interval = 0;
			if (GetNextScheduledTime(ref interval, Now))
			{
				_waiter = new ManualResetEvent(false);

				ThreadPool.RegisterWaitForSingleObject(_waiter, WaitExpired, interval, interval, true);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nextScheduledTime"></param>
		/// <param name="now"></param>
		/// <returns>True if there is a future time to schedule, 
		/// otherwise false if nothing is available or execution is immediately required</returns>
		public bool GetNextScheduledTime(ref long nextScheduledTime, long now)
		{
			nextScheduledTime = 0;

			if (_pending.Count <= 0)
				return false;

			foreach (var pair in _pending)
			{
				if (now >= pair.Key)
					return false;

				nextScheduledTime = (pair.Key - now);
				return true;
			}

			return false;
		}

		private void Queue(IEnumerable<ScheduledEvent> rescheduled)
		{
			if (rescheduled == null) return;

			foreach (var pendingEvent in rescheduled)
			{
				QueueEvent(pendingEvent);
			}
		}

		private List<ScheduledEvent> ExecuteExpired()
		{
			var expired = RemoveExpired();

			List<ScheduledEvent> rescheduled = null;
			if (expired.Count <= 0)
				return rescheduled;

			foreach (var pair in expired)
			{
				foreach (var pendingEvent in pair.Value)
				{
					var next = pendingEvent.Execute(Now);
					if (next == null) continue;

					if (rescheduled == null)
						rescheduled = new List<ScheduledEvent>(2);

					rescheduled.Add(next);
				}
			}
			return rescheduled;
		}

		private SortedList<long, List<ScheduledEvent>> RemoveExpired()
		{
			lock (_lock)
			{
				var expired = new SortedList<long, List<ScheduledEvent>>();
				foreach (var item in _pending)
				{
					if (Now < item.Key)
						break;

					expired.Add(item.Key, item.Value);
				}

				foreach (var item in expired)
				{
					_pending.Remove(item.Key);
				}

				return expired;
			}
		}
	}
}