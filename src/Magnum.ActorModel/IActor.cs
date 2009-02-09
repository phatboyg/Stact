namespace Magnum.ActorModel
{
	using System;

	public interface IActor
	{
		void Enqueue(Action action);
		void EnqueueAll(params Action[] actions);
	}
}