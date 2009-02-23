namespace Magnum.ActorModel.WebSpecs.Simple
{
	using System;
	using System.Text;
	using System.Threading;
	using System.Web;
	using Threading;

	public class SimpleRequestActor :
		AsyncHttpActor
	{
		private static int _requestCount;
		private static int _responseCount;
		protected readonly CommandQueue _queue;
		private readonly IRequestService _requestService;
		private AsyncResult _asyncResult;

		private StringBuilder _body = new StringBuilder();
		private HttpContext _context;

		public SimpleRequestActor(CommandQueue queue, IRequestService requestService)
		{
			_queue = queue;
			_requestService = requestService;

			_body.AppendLine("Actor created on thread: " + Thread.CurrentThread.ManagedThreadId);
		}

		public IAsyncResult BeginAction(HttpContext context, AsyncCallback callback, object state)
		{
			_context = context;
			_asyncResult = new AsyncResult(callback, state);

			_body.AppendLine("Begin called on thread: " + Thread.CurrentThread.ManagedThreadId);

			var message = new SimpleRequest
				{
					Reply = response => _queue.Enqueue(() => Consume(response))
				};

			Interlocked.Increment(ref _requestCount);
			_requestService.SimpleRequest(message);

			return _asyncResult;
		}

		private void Consume(SimpleResponse response)
		{
			Interlocked.Increment(ref _responseCount);

			_body.AppendLine("Response created at: " + response.Created);
			_body.AppendLine(response.Message);
			_body.AppendLine("Callback executed on thread: " + Thread.CurrentThread.ManagedThreadId);
			_body.AppendLine("Request Count: " + _requestCount + " Response Count: " + _responseCount);

			_context.Response.ContentType = "text/plain";
			_context.Response.Write(_body.ToString());

			_asyncResult.SetAsCompleted();
		}
	}
}