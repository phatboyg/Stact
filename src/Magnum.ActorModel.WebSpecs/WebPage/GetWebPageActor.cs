namespace Magnum.ActorModel.WebSpecs.WebPage
{
	using System;
	using System.Text;
	using System.Threading;
	using System.Web;
	using Threading;

	public class GetWebPageActor :
		AsyncHttpActor
	{
		private readonly CommandQueue _queue;
		private readonly WebPageRetrieval _webPageRetrieval;
		private static int _requestCount;
		private static int _responseCount;
		private readonly StringBuilder _body = new StringBuilder();
		private HttpContext _context;
		private AsyncResult _asyncResult;

		public GetWebPageActor(CommandQueue queue, WebPageRetrieval webPageRetrieval)
		{
			_queue = queue;
			_webPageRetrieval = webPageRetrieval;
			_body.AppendLine("Actor created on thread: " + Thread.CurrentThread.ManagedThreadId);
		}

		public IAsyncResult BeginAction(HttpContext context, AsyncCallback callback, object state)
		{
			_context = context;
			_asyncResult = new AsyncResult(callback, state);

			_body.AppendLine("Begin called on thread: " + Thread.CurrentThread.ManagedThreadId);

			var request = new GetWebPage
				{
					Url = new Uri("http://www.google.com/"),
					Reply = response => _queue.Enqueue(() => Consume(response)),
				};

			Interlocked.Increment(ref _requestCount);
			_webPageRetrieval.GetWebPage(request);

			return _asyncResult;
		}

		private void Consume(WebPageContent response)
		{
			Interlocked.Increment(ref _responseCount);

			_body.AppendLine(response.RequestStarted);
			_body.AppendLine(response.RequestCompleted);
			_body.AppendLine("Callback executed on thread: " + Thread.CurrentThread.ManagedThreadId);
			_body.AppendLine("Request Count: " + _requestCount + " Response Count: " + _responseCount);
			_body.AppendLine("Length of received content: " + response.Html.Length);
			_body.AppendLine();
			_body.AppendLine(response.Html.Substring(0, 128));

			_context.Response.ContentType = "text/plain";
			_context.Response.Write(_body.ToString());

			_asyncResult.SetAsCompleted();
		}
	}
}