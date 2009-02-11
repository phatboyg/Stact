namespace Magnum.ActorModel
{
	using System;

	public interface CommandContext :
		CommandQueue,
		IDisposable
	{
		void Start();
	}
}