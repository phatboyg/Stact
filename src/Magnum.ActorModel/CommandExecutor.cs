namespace Magnum.ActorModel
{
	using System;

	/// <summary>
	/// Used to execute commands
	/// </summary>
	public interface CommandExecutor
	{
		/// <summary>
		/// Execute the action
		/// </summary>
		/// <param name="toExecute"></param>
		void Execute(Action toExecute);

		/// <summary>
		/// Execute the actions 
		/// </summary>
		/// <param name="toExecute"></param>
		void ExecuteAll(Action[] toExecute);

		/// <summary>
		/// Disables the executor so that any future Execute calls are ignored
		/// </summary>
		void Disable();

		/// <summary>
		/// True if the executor is enabled
		/// </summary>
		bool IsEnabled { get; }
	}
}