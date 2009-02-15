namespace Magnum.ActorModel.WebSpecs.WebPage
{
	using System;

	public class WebPageContent
	{
		public WebPageContent()
		{
			CorrelationId = CombGuid.Generate();
		}

		public string RequestStarted { get; set; }

		public Guid CorrelationId { get; set; }

		public Uri Url { get; set; }

		public string Html { get; set; }

		public string ContentType { get; set; }

		public string RequestCompleted { get; set; }
	}
}