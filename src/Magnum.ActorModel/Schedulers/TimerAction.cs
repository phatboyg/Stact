namespace Magnum.ActorModel.Schedulers
{
	using System;
	using System.Threading;

	public class TimerAction :
		IDisposable
	{
		private readonly Action _action;
		private readonly long _initialInterval;
		private readonly long _periodicInterval;

		private bool _disposed;
		private bool _enabled = true;
		private Timer _timer;

		public TimerAction(long initialInterval, long periodicInterval, Action action)
		{
			_action = action;
			_initialInterval = initialInterval;
			_periodicInterval = periodicInterval;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Schedule(SchedulerControl control)
		{
			if (_timer != null)
				throw new InvalidOperationException("An action is already scheduled for this TimerAction");

			TimerCallback callback = delegate { TimerIntervalCallback(control); };
			_timer = new Timer(callback, null, _initialInterval, _periodicInterval);
		}

		public void TimerIntervalCallback(SchedulerControl control)
		{
			if (_periodicInterval == Timeout.Infinite || !_enabled)
			{
				control.Remove(this);
				if (_timer != null)
				{
					_timer.Dispose();
					_timer = null;
				}
			}

			if (_enabled)
			{
				control.Enqueue(ExecuteAction);
			}
		}

		public void ExecuteAction()
		{
			if (_enabled)
			{
				_action();
			}
		}

		public virtual void Cancel()
		{
			_enabled = false;
		}

		~TimerAction()
		{
			Dispose(false);
		}

		public void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_timer != null)
				{
					_timer.Dispose();
					_timer = null;
				}
			}
			_disposed = true;
		}
	}
}