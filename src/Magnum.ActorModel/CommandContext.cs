namespace Magnum.ActorModel
{
	public interface CommandContext :
		CommandQueue,
		Scheduler
	{
		void Start();
	}
}