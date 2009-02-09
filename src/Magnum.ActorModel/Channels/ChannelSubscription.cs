namespace Magnum.ActorModel.Channels
{
	using System;

	public class ChannelSubscription<T> :
		SubscriptionBase<T>
	{
		private readonly IActor _actor;
		private readonly Action<T> _consume;

		public ChannelSubscription(IActor actor, Action<T> consume)
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