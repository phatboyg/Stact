namespace Magnum.ActorModel
{
	using System;

	public class SynchronousCommandQueue :
		CommandQueue
	{
		private bool _enabled = true;

		public void EnqueueAll(params Action[] actions)
		{
			if (!_enabled) return;

			foreach (var action in actions)
			{
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
