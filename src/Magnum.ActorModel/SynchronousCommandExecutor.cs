namespace Magnum.ActorModel
{
	using System;

	public class SynchronousCommandExecutor :
		CommandExecutor
	{
		private bool _enabled = true;

		public bool IsEnabled
		{
			get { return _enabled; }
		}

		public void ExecuteAll(Action[] toExecute)
		{
			foreach (var action in toExecute)
			{
				Execute(action);
			}
		}

		public void Execute(Action toExecute)
		{
			if (_enabled)
			{
				toExecute();
			}
		}

		public void Disable()
		{
			_enabled = false;
		}
	}
}