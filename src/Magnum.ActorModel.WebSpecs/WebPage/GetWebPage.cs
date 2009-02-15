namespace Magnum.ActorModel.WebSpecs.WebPage
{
	using System;

	public class GetWebPage
	{
		public GetWebPage()
		{
			CorrelationId = CombGuid.Generate();
		}

		public Guid CorrelationId { get; set; }

		public Uri Url { get; set; }
	}
}