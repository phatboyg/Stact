namespace Magnum.ActorModel.WebSpecs.WebPage
{
	using System.Net;
	using System.Threading;

	public interface WebPageRetrieval
	{
		void GetWebPage(GetWebPage getWebPage);
	}

	public class WebPageRetrievalService :
		WebPageRetrieval,
		IStartable
	{
		private readonly CommandQueue _queue;

		public WebPageRetrievalService(CommandQueue queue)
		{
			_queue = queue;
		}

		public void Start()
		{
		}

		public void GetWebPage(GetWebPage getWebPage)
		{
			_queue.Enqueue(() => Consume(getWebPage));
		}

		public void Consume(GetWebPage message)
		{
			var content = new WebPageContent
				{
					RequestStarted = "Request initiated on thread: " + Thread.CurrentThread.ManagedThreadId,
				};

			var request = WebRequest.Create(message.Url);

			request.GetResponseAsync()(response => response.GetResponseStream().ReadToEndAsync()(html =>
				{
					content.Html = html;
					content.ContentType = response.ContentType;
					content.RequestCompleted = "Request completed on thread: " + Thread.CurrentThread.ManagedThreadId;

					message.Reply(content);
				}));
		}
	}
}