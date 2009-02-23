namespace Magnum.ActorModel.CommandQueues
{
	using System;

	/// <summary>
	/// Implements a command queues that calls actions immediately on the callers thread
	/// as they are queued. This should only be used for testing and not in production.
	/// </summary>
	public class SynchronousCommandQueue :
		CommandQueue
	{
		private bool _enabled = true;

		public void EnqueueAll(params Action[] actions)
		{
			if (!_enabled) return;

			foreach (var action in actions)
			{
				if (!_enabled)
					break;

				action();
			}
		}

		public void Enqueue(Action action)
		{
			if (_enabled)
			{
				action();
			}
		}

		public void Run()
		{
			_enabled = true;
		}

		public void Disable()
		{
			_enabled = false;
		}
	}
}