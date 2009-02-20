namespace Magnum.ActorModel.Channels.Subscribers
{
	using System;

	public class ChannelSubscriber<T> :
		SubscriberBase<T>
	{
		private readonly CommandQueue _queue;
		private readonly Action<T> _consume;

		public ChannelSubscriber(CommandQueue queue, Action<T> consume)
		{
			_queue = queue;
			_consume = consume;
		}

		protected override void ConsumeMessage(T message)
		{
			_queue.Enqueue(() => _consume(message));
		}
	}
}