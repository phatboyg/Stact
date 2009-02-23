namespace Magnum.ActorModel.WebSpecs.Simple
{
	using System.Threading;

	public interface IRequestService
	{
		void SimpleRequest(SimpleRequest request);
	}

	public class RequestService :
		IRequestService,
		IStartable
	{
		private readonly CommandQueue _queue;

		public RequestService(CommandQueue queue)
		{
			_queue = queue;
		}

		public void SimpleRequest(SimpleRequest request)
		{
			_queue.Enqueue(() => Consume(request));
		}

		public void Start()
		{
		}

		public void Consume(SimpleRequest message)
		{
			var response = new SimpleResponse
				{
					Message = "Response created on thread: " + Thread.CurrentThread.ManagedThreadId,
				};

			message.Reply(response);
		}
	}
}