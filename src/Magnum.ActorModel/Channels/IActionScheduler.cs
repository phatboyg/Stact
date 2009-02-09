namespace Magnum.ActorModel.Channels
{
	using System;

	public interface IActionScheduler
	{
		void Schedule(int interval, Action action);
	}
}