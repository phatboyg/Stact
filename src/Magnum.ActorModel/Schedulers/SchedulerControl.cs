namespace Magnum.ActorModel.Schedulers
{
	using System;

	public interface SchedulerControl
	{
		void Remove(TimerAction timerAction);
		void Enqueue(Action action);
	}
}