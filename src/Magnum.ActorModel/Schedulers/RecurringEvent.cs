namespace Magnum.ActorModel.Schedulers
{
	using System;

	public class RecurringEvent :
		EventBase
	{
		private readonly long _periodicInterval;

		public RecurringEvent(long initialInterval, long periodicInterval, Action action, long now)
		{
			_periodicInterval = periodicInterval;
			_action = action;
			_scheduledTime = now + initialInterval;
		}

		public override ScheduledEvent Execute(long now)
		{
			if (_cancelled) return null;

			_action();
			_scheduledTime = now + _periodicInterval;
			return this;
		}
	}
}