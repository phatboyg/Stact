namespace Magnum.ActorModel.WebSpecs.Simple
{
	using System;
	using System.Text;
	using System.Threading;
	using System.Web;
	using Threading;

	public class SimpleRequestActor :
		ActorBase,
		AsyncActor
	{
		private static int _requestCount;
		private static int _responseCount;
		private readonly HttpContext _context;
		private AsyncResult _asyncResult;

		private StringBuilder _body = new StringBuilder();

		public SimpleRequestActor(HttpContext context, CommandQueue queue, ChannelFactory channelFactory)
			: base(queue, channelFactory)
		{
			_context = context;
			_body.AppendLine("Actor created on thread: " + Thread.CurrentThread.ManagedThreadId);
		}

		public IAsyncResult Begin(AsyncCallback callback, object state)
		{
			_asyncResult = new AsyncResult(callback, state);

			_body.AppendLine("Begin called on thread: " + Thread.CurrentThread.ManagedThreadId);

			When<SimpleResponse>(filter => filter.CorrelationId == TransactionId, response =>
				{
					Interlocked.Increment(ref _responseCount);

					_body.AppendLine("Response created at: " + response.Created);
					_body.AppendLine(response.Message);
					_body.AppendLine("Callback executed on thread: " + Thread.CurrentThread.ManagedThreadId);
					_body.AppendLine("Request Count: " + _requestCount + " Response Count: " + _responseCount);

					_context.Response.ContentType = "text/plain";
					_context.Response.Write(_body.ToString());

					_asyncResult.SetAsCompleted();
				});

			var message = new SimpleRequest {CorrelationId = TransactionId};

			Interlocked.Increment(ref _requestCount);
			Publish(message);

			return _asyncResult;
		}
	}
}