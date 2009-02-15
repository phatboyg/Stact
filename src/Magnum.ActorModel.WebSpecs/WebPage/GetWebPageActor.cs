namespace Magnum.ActorModel.WebSpecs.WebPage
{
	using System;
	using System.Text;
	using System.Threading;
	using System.Web;
	using Threading;

	public class GetWebPageActor :
		ActorBase,
		AsyncActor
	{
		private static int _requestCount;
		private static int _responseCount;
		private readonly StringBuilder _body = new StringBuilder();
		private readonly HttpContext _context;
		private AsyncResult _asyncResult;

		public GetWebPageActor(HttpContext context, CommandQueue queue, ChannelFactory channelFactory)
			: base(queue, channelFactory)
		{
			_context = context;
			_body.AppendLine("Actor created on thread: " + Thread.CurrentThread.ManagedThreadId);
		}

		public IAsyncResult Begin(AsyncCallback callback, object state)
		{
			_asyncResult = new AsyncResult(callback, state);

			_body.AppendLine("Begin called on thread: " + Thread.CurrentThread.ManagedThreadId);

			When<WebPageContent>(filter => filter.CorrelationId == TransactionId, response =>
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
				});

			var request = new GetWebPage {CorrelationId = TransactionId, Url = new Uri("http://www.google.com/")};

			Interlocked.Increment(ref _requestCount);
			Publish(request);

			return _asyncResult;
		}
	}
}