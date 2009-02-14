namespace Magnum.ActorModel
{
	using System;
	using Channels;
	using Channels.Subscribers;
	using Monads;

	public class ActorBase
	{
		private readonly ChannelFactory _channelFactory;
		private readonly CommandQueue _queue;

		public ActorBase(CommandQueue queue, ChannelFactory channelFactory)
			: this(queue, channelFactory, CombGuid.Generate())
		{
		}

		public ActorBase(CommandQueue queue, ChannelFactory channelFactory, Guid transactionId)
		{
			_queue = queue;
			_channelFactory = channelFactory;
			TransactionId = transactionId;
		}

		public Guid TransactionId { get; private set; }

		protected void Publish<T>(T message)
		{
			var channel = _channelFactory.GetChannel<T>();
			channel.Publish(message);
		}

		protected void When<T>(Filter<T> filter, Action<T> continuation)
		{
			K<T> result = respond =>
				{
					var channel = _channelFactory.GetChannel<T>();

					Unsubscribe unsubscribe = null;
					unsubscribe = channel.Subscribe(_queue, response =>
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