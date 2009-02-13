namespace Magnum.ActorModel.WebSpecs
{
	using System.Threading;
	using Channels;

	public class RequestService :
		IStartable
	{
		private readonly CommandContext _queue;
		private readonly Channel<SimpleRequest> _requestChannel;
		private readonly Channel<SimpleResponse> _responseChannel;

		public RequestService(CommandContext queue, Channel<SimpleRequest> requestChannel, Channel<SimpleResponse> responseChannel)
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

		public void Consume(SimpleRequest message)
		{
			var response = new SimpleResponse
				{
					CorrelationId = message.CorrelationId,
					Message = "Response created on thread: " + Thread.CurrentThread.ManagedThreadId,
				};

			_responseChannel.Publish(response);
		}
	}
}