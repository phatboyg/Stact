namespace Magnum.ActorModel
{
	using System;

	public interface CommandExecutor
	{
		void ExecuteAll(Action[] toExecute);
		void Execute(Action toExecute);
		void Disable();
	}
}