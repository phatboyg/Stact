namespace Magnum.ActorModel.WebSpecs
{
	using System;

	public class SimpleRequest
	{
		public SimpleRequest()
		{
			CorrelationId = CombGuid.Generate();
		}

		public Guid CorrelationId { get; set; }
	}
}