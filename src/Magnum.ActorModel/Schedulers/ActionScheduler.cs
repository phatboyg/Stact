namespace Magnum.ActorModel.Schedulers
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	public class ActionScheduler :
		Scheduler,
		SchedulerControl
	{
		private readonly CommandQueue _queue;
		private volatile bool _enabled = true;
		private List<TimerAction> _pending = new List<TimerAction>();

		public ActionScheduler(CommandQueue queue)
		{
			_queue = queue;
		}

		public void Dispose()
		{
			_enabled = false;
			var pendingActions = Interlocked.Exchange(ref _pending, new List<TimerAction>());
			foreach (TimerAction timerAction in pendingActions)
			{
				timerAction.Cancel();
			}
		}

		public Unschedule Schedule(int interval, Action action)
		{
			if (interval <= 0)
			{
				var pending = new PendingAction(action);
				_queue.Enqueue(pending.Execute);

				return pending.Cancel;
			}

			return Schedule(interval, Timeout.Infinite, action);
		}

		public Unschedule Schedule(int initialInterval, int periodicInterval, Action action)
		{
			var pending = new TimerAction(initialInterval, periodicInterval, action);
			Add(pending);

			return pending.Cancel;
		}

		public void Remove(TimerAction timerAction)
		{
			_queue.Enqueue(() => _pending.Remove(timerAction));
		}

		public void Enqueue(Action action)
		{
			_queue.Enqueue(action);
		}

		private void Add(TimerAction pending)
		{
			_queue.Enqueue(() =>
				{
					if (!_enabled) return;

					_pending.Add(pending);
					pending.Schedule(this);
				});
		}
	}
}