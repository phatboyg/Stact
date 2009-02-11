namespace Magnum.ActorModel.Schedulers
{
	using System;

	public abstract class EventBase :
		ScheduledEvent
	{
		protected Action _action;
		protected bool _cancelled;
		protected long _scheduledTime;

		public long ScheduledTime
		{
			get { return _scheduledTime; }
		}

		public abstract ScheduledEvent Execute(long now);

		public void Cancel()
		{
			_cancelled = true;
		}
	}
}