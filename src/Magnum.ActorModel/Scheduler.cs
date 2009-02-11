namespace Magnum.ActorModel
{
	using System;

	public delegate void Unschedule();

	public interface Scheduler :
		IDisposable
	{
		Unschedule Schedule(int interval, Action action);
		Unschedule Schedule(int initialInterval, int periodicInterval, Action action);
	}
}