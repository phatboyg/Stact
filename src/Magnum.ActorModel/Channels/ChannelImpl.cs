namespace Magnum.ActorModel.Channels
{
	using System;

	public class ChannelImpl<T> :
		Channel<T>
	{
		public bool Publish(T message)
		{
			var subscribers = _subscribers;
			if (subscribers != null)
			{
				subscribers(message);
				return true;
			}
			return false;
		}

		public Unsubscribe Subscribe(Action<T> consumer)
		{
			_subscribers += consumer;

			return () => _subscribers -= consumer;
		}

		public void UnsubscribeAll()
		{
			_subscribers = null;
		}

		private event Action<T> _subscribers;
	}
}