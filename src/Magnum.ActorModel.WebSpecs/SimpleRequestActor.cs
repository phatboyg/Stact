namespace Magnum.ActorModel.WebSpecs
{
	using System;
	using System.Text;
	using System.Threading;
	using System.Web;
	using Channels;
	using Channels.Subscribers;
	using CommandQueues;
	using Monads;
	using StructureMap;
	using Threading;

	public class SimpleRequestActor :
		Actor
	{
		private static int _requestCount;
		private static int _responseCount;
		private readonly HttpContext _context;
		private AsyncResult _asyncResult;

		private StringBuilder _body = new StringBuilder();

		public SimpleRequestActor(HttpContext context)
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

	public class Actor
	{
		public Actor()
			: this(CombGuid.Generate())
		{
		}

		public Actor(Guid transactionId)
		{
			TransactionId = transactionId;
		}

		public Guid TransactionId { get; private set; }

		protected static void Publish<T>(T message)
		{
			var channel = ObjectFactory.GetInstance<Channel<T>>();
			channel.Publish(message);
		}

		protected static void When<T>(Filter<T> filter, Action<T> continuation)
		{
			K<T> result = respond =>
				{
					var queue = ObjectFactory.GetInstance<ThreadPoolCommandQueue>();

					var channel = ObjectFactory.GetInstance<Channel<T>>();

					Unsubscribe unsubscribe = null;
					unsubscribe = channel.Subscribe(queue, response =>
						{
							try
							{
								respond(response);
							}
							finally
							{
								unsubscribe();
							}
						}, filter);
				};

			result(continuation);
		}
	}
}