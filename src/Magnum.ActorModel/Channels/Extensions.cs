namespace Magnum.ActorModel.Channels
{
	using System;
	using System.Collections.Generic;
	using Subscribers;

	public static class Extensions
	{
		public static Unsubscribe Subscribe<T>(this Channel<T> channel, CommandQueue queue, Action<T> consume)
		{
			return channel.Subscribe(message => queue.Enqueue(() => consume(message)));
		}

		public static Unsubscribe Subscribe<T>(this Channel<T> channel, Action<T> consume, int interval, Scheduler scheduler)
		{
			var subscriber = new LatestIntervalSubscriber<T>(consume, interval, scheduler);
			return channel.Subscribe(subscriber.Consume);
		}

		public static Unsubscribe Subscribe<T>(this Channel<T> channel, CommandQueue queue, Action<T> consume, Filter<T> filter)
		{
			var subscriber = new FilteredSubscriber<T>(queue, consume, filter);
			return channel.Subscribe(subscriber.Consume);
		}

		public static Unsubscribe Subscribe<T>(this Channel<T> channel, Action<IList<T>> consume, int interval, Scheduler scheduler)
		{
			var subscriber = new IntervalSubscriber<T>(consume, interval, scheduler);
			return channel.Subscribe(subscriber.Consume);
		}

		public static Unsubscribe Subscribe<T, K>(this Channel<T> channel, Action<IDictionary<K, T>> consume, int interval, Converter<T, K> keyConverter, Scheduler scheduler)
		{
			var subscriber = new DistinctIntervalSubscriber<K, T>(consume, interval, keyConverter, scheduler);
			return channel.Subscribe(subscriber.Consume);
		}
	}
}