namespace Magnum.ActorModel
{
	public interface ScheduledEvent
	{
		long ScheduledTime { get; }
		ScheduledEvent Execute(long now);
		void Cancel();
	}
}