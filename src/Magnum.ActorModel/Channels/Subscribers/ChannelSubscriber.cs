namespace Magnum.ActorModel.Channels.Subscribers
{
	using System;

	public class ChannelSubscriber<T> :
		SubscriberBase<T>
	{
		private readonly CommandQueue _actor;
		private readonly Action<T> _consume;

		public ChannelSubscriber(CommandQueue actor, Action<T> consume)
		{
			_actor = actor;
			_consume = consume;
		}

		protected override void ConsumeMessage(T message)
		{
			_actor.Enqueue(() => _consume(message));
		}
	}
}