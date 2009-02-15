namespace Magnum.ActorModel.WebSpecs.WebPage
{
	using System.Net;
	using System.Threading;
	using Channels;

	public class WebPageRetrievalService :
		IStartable
	{
		private readonly CommandContext _queue;
		private readonly Channel<GetWebPage> _requestChannel;
		private readonly Channel<WebPageContent> _responseChannel;

		public WebPageRetrievalService(CommandContext queue,
		                               Channel<GetWebPage> requestChannel,
		                               Channel<WebPageContent> responseChannel)
		{
			_queue = queue;
			_responseChannel = responseChannel;
			_requestChannel = requestChannel;
		}

		public void Start()
		{
			_requestChannel.Subscribe(_queue, Consume);

			_queue.Start();
		}

		public void Consume(GetWebPage message)
		{
			var content = new WebPageContent
			              	{
			              		CorrelationId = message.CorrelationId,
			              		RequestStarted = "Request initiated on thread: " + Thread.CurrentThread.ManagedThreadId,
			              	};

			var request = WebRequest.Create(message.Url);

			request.GetResponseAsync()(response => response.GetResponseStream().ReadToEndAsync()(html =>
				{
					content.Html = html;
					content.ContentType = response.ContentType;
					content.RequestCompleted = "Request completed on thread: " + Thread.CurrentThread.ManagedThreadId;

					_responseChannel.Publish(content);
				}));
		}
	}
}