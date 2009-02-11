namespace Magnum.ActorModel.Schedulers
{
	public interface ScheduledEvent
	{
		ScheduledEvent Execute(long now);
		long ScheduledTime { get; }
		void Cancel();
	}
}