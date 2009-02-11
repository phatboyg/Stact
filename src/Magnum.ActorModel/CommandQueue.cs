namespace Magnum.ActorModel
{
	using System;

	/// <summary>
	/// A queue-backed execution context to ensure all operations are done on the actors thread
	/// </summary>
	public interface CommandQueue
	{
		/// <summary>
		/// Adds an action to the end of the command queue
		/// </summary>
		/// <param name="action"></param>
		void Enqueue(Action action);

		/// <summary>
		/// Adds a range of actions to the end of the command queue
		/// </summary>
		/// <param name="actions"></param>
		void EnqueueAll(params Action[] actions);

		/// <summary>
		/// Disables the command queue, losing all commands that are present in the queue
		/// </summary>
		void Disable();

		/// <summary>
		/// Runs the command queue on the callers thread. Does not return until the queue is
		/// disabled.
		/// </summary>
		void Run();
	}
}