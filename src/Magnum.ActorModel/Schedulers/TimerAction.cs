namespace Magnum.ActorModel
{
	using System;
	using System.Threading;

	public class TimerAction : 
		IDisposable
	{
		private readonly Action _action;
		private readonly long _initialInterval;
		private readonly long _periodicInterval;

		private Timer _timer;
		private bool _enabled = true;

		public TimerAction(long initialInterval, long periodicInterval, Action action)
		{
			_action = action;
			_initialInterval = initialInterval;
			_periodicInterval = periodicInterval;
		}

		public void Schedule(SchedulerControl control)
		{
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

		public void Dispose()
		{
			if(_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}
	}
}