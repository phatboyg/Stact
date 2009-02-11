namespace Magnum.ActorModel
{
	using System;

	public class PendingAction
	{
		private readonly Action _action;
		private bool _enabled = true;

		public PendingAction(Action action)
		{
			_action = action;
		}

		public void Execute()
		{
			if (!_enabled) return;

			_action();
		}

		public void Cancel()
		{
			_enabled = false;
		}
	}
}