namespace Magnum.ActorModel
{
	public interface ScheduledEvent
	{
		ScheduledEvent Execute(long now);
		long ScheduledTime { get; }
		void Cancel();
	}
}