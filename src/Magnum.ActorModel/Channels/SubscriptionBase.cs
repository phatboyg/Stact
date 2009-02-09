namespace Magnum.ActorModel.Channels
{
	public delegate bool Filter<T>(T message);

	public abstract class SubscriptionBase<T>
	{
		private readonly Filter<T> _filter;

		protected SubscriptionBase()
		{
		}

		protected SubscriptionBase(Filter<T> filter)
		{
			_filter = filter;
		}

		private bool IsSatisfiedByFilter(T msg)
		{
			return _filter == null || _filter(msg);
		}

		public void Consume(T message)
		{
			if (IsSatisfiedByFilter(message))
			{
				ConsumeMessage(message);
			}
		}

		protected abstract void ConsumeMessage(T message);
	}
}