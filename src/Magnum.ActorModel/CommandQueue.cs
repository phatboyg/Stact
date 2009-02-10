namespace Magnum.ActorModel
{
	using System;

	public interface CommandQueue
	{
		void Enqueue(Action action);
		void EnqueueAll(params Action[] actions);
		void Disable();
		void Run();
	}
}