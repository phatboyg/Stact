namespace Magnum.ActorModel
{
	using System;

	public interface ICommandExecutor
	{
		void ExecuteAll(Action[] toExecute);
		void Execute(Action toExecute);
	}
}