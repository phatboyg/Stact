namespace Magnum.ActorModel.WebSpecs
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using System.Web;
	using Channels;
	using Channels.Subscribers;
	using CommandQueues;
	using Monads;
	using StructureMap;

	public class BasicAsyncHandler :
		IHttpAsyncHandler
	{
		private ThreadPoolCommandQueue _queue;
		private Channel<SimpleRequest> _requestChannel;
		private Channel<SimpleResponse> _responseChannel;
		private static int _requestCount;
		private static int _responseCount;

		public BasicAsyncHandler()
		{
			_requestChannel = ObjectFactory.GetInstance<Channel<SimpleRequest>>();
			_responseChannel = ObjectFactory.GetInstance<Channel<SimpleResponse>>();

			_queue = ObjectFactory.GetInstance<ThreadPoolCommandQueue>();
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new InvalidOperationException("This should not be called since we are an asynchronous handler");
		}

		// Since the handler itself is stateless, let ASP.NET reuse our class for performance reasons
		public bool IsReusable
		{
			get { return true; }
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object state)
		{
			HttpHandlerAsyncResult ar = new HttpHandlerAsyncResult(context, callback, state);

			string requestThreadInfo = "Request created on thread: " + Thread.CurrentThread.ManagedThreadId;

			Guid transactionId = CombGuid.Generate();

			var x = WithResponse(_queue, response => response.CorrelationId == transactionId);
			Trace.WriteLine("Defining Response Handler");
			x(response =>
				{
					Interlocked.Increment(ref _responseCount);
					context.Response.ContentType = "text/plain";
					context.Response.Write(requestThreadInfo);
					context.Response.Write("\nThis response was created just for you at " + response.Created);
					context.Response.Write("\nResponse Message: " + response.Message);
					context.Response.Write("\nCallback execute on thread: " + Thread.CurrentThread.ManagedThreadId);

					context.Response.Write("\nRequest Count: " + _requestCount + " Response Count: " + _responseCount);

					ar.SetAsCompleted();
				});


			var message = new SimpleRequest {CorrelationId = transactionId};
			Trace.WriteLine("Publishing request");
			Interlocked.Increment(ref _requestCount);
			_requestChannel.Publish(message);

			Trace.WriteLine("returning");
			return ar;
		}


		public void EndProcessRequest(IAsyncResult result)
		{
			HttpHandlerAsyncResult ar = result as HttpHandlerAsyncResult;
			if (ar == null)
				throw new InvalidOperationException("We didn't get our state back, so we explode");

			ar.Context.Response.Write("\nIn EndProcessRequest on thread: " + Thread.CurrentThread.ManagedThreadId);
		}

		public K<SimpleResponse> WithResponse(CommandQueue queue, Filter<SimpleResponse> filter)
		{
			return respond =>
				{
					Trace.WriteLine("Subscribing to channel");
					Unsubscribe unsubscribe = null;
					unsubscribe = _responseChannel.Subscribe(queue, response =>
						{
							unsubscribe();
							respond(response);
						}, filter);
				};
		}
	}
}