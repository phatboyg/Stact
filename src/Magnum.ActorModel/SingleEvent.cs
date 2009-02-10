namespace Magnum.ActorModel
{
	using System;

	public class SingleEvent :
		ScheduledEvent
	{
		private readonly Action _action;
		private readonly CommandQueue _actor;
		private readonly long _scheduledTime;
		private bool _cancelled;

		public SingleEvent(CommandQueue actor, Action action, long waitTime, long now)
		{
			_actor = actor;
			_action = action;
			_scheduledTime = now + waitTime;
		}

		public long ScheduledTime
		{
			get { return _scheduledTime; }
		}

		public ScheduledEvent Execute(long now)
		{
			if (_cancelled) return null;

			_actor.Enqueue(_action);

			return null;
		}

		public void Cancel()
		{
			_cancelled = true;
		}
	}
}