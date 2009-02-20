namespace Magnum.ActorModel.WebSpecs
{
	using System;

	public interface ExampleRequest
	{
		long Number { get; }

		Action<long> Continue { get; }

		Action<Exception> Complain { get; }

		
	}
}