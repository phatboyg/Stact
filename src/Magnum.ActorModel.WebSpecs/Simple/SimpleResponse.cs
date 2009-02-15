namespace Magnum.ActorModel.WebSpecs.Simple
{
	using System;

	public class SimpleResponse
	{
		public SimpleResponse()
		{
			Created = DateTime.Now;
		}

		public Guid CorrelationId { get; set; }
		public string Message { get; set; }
		public DateTime Created { get; set; }
	}
}