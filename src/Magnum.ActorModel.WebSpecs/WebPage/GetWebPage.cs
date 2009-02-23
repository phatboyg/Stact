namespace Magnum.ActorModel.WebSpecs.WebPage
{
	using System;

	public class GetWebPage
	{
		public Uri Url { get; set; }

		public Action<WebPageContent> Reply { get; set; }
	}
}