namespace Magnum.ActorModel.Channels.Subscribers
{
	using System;

	public class FilteredSubscriber<T> :
		SubscriberBase<T>
	{
		private readonly CommandQueue _actor;
		private readonly Action<T> _consume;

		public FilteredSubscriber(CommandQueue actor, Action<T> consume, Filter<T> filter)
			: base(filter)
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