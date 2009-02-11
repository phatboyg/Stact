namespace Magnum.ActorModel.Schedulers
{
	using System;

	public class SingleEvent :
		EventBase
	{
		public SingleEvent(long interval, Action action, long now)
		{
			_action = action;
			_scheduledTime = now + interval;
		}

		public override ScheduledEvent Execute(long now)
		{
			if (_cancelled) return null;

			_action();

			return null;
		}
	}
}