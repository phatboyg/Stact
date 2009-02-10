namespace Magnum.ActorModel.Channels
{
	using System;

	public delegate void Unschedule();

	public interface Scheduler
	{
		Unschedule Schedule(int interval, Action action);
	}
}