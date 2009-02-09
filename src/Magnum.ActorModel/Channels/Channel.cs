namespace Magnum.ActorModel.Channels
{
	using System;
	using System.Collections.Generic;

	public class Channel<T> :
		IChannel<T>
	{
//        public IUnsubscriber Subscribe(IDisposingExecutor executor, Action<T> receive)
//        {
//            var subscriber = new ChannelSubscription<T>(executor, receive);
//            return SubscribeOnProducerThreads(subscriber);
//        }
//
//        internal void Unsubscribe(Action<T> toUnsubscribe)
//        {
//            _subscribers -= toUnsubscribe;
//        }
//
//        /// <summary>
//        /// <see cref="IPublisher{T}.Publish(T)"/>
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <returns></returns>

//        /// <summary>
//        /// <see cref="ISubscriber{T}.SubscribeToBatch(IScheduler,Action{IList{T}},int)"/>
//        /// </summary>
//        /// <param name="scheduler"></param>
//        /// <param name="receive"></param>
//        /// <param name="intervalInMs"></param>
//        /// <returns></returns>
//        public IUnsubscriber SubscribeToBatch(IScheduler scheduler, Action<IList<T>> receive, int intervalInMs)
//        {
//            var batch = new BatchSubscriber<T>(scheduler, receive, intervalInMs);
//            return SubscribeOnProducerThreads(batch);
//        }
//
//        /// <summary>
//        /// <see cref="ISubscriber{T}.SubscribeToKeyedBatch{K}(IScheduler,Converter{T,K},Action{IDictionary{K,T}},int)"/>
//        /// </summary>
//        /// <typeparam name="K"></typeparam>
//        /// <param name="scheduler"></param>
//        /// <param name="keyResolver"></param>
//        /// <param name="receive"></param>
//        /// <param name="intervalInMs"></param>
//        /// <returns></returns>
//        public IUnsubscriber SubscribeToKeyedBatch<K>(IScheduler scheduler,
//                                                      Converter<T, K> keyResolver, Action<IDictionary<K, T>> receive,
//                                                      int intervalInMs)
//        {
//            var batch = new KeyedBatchSubscriber<K, T>(keyResolver, receive, scheduler, intervalInMs);
//            return SubscribeOnProducerThreads(batch);
//        }
//
//        /// <summary>
//        /// Subscription that delivers the latest message to the consuming thread.  If a newer message arrives before the consuming thread
//        /// has a chance to process the message, the pending message is replaced by the newer message. The old message is discarded.
//        /// </summary>
//        /// <param name="scheduler"></param>
//        /// <param name="receive"></param>
//        /// <param name="intervalInMs"></param>
//        /// <returns></returns>
//        public IUnsubscriber SubscribeToLast(IScheduler scheduler, Action<T> receive, int intervalInMs)
//        {
//            var sub = new LastSubscriber<T>(receive, scheduler, intervalInMs);
//            return SubscribeOnProducerThreads(sub);
//        }

		public void UnsubscribeAll()
		{
			_subscribers = null;
		}

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

		public Unsubscribe Subscribe(IActor actor, Action<T> consume)
		{
			var subscriber = new ChannelSubscription<T>(actor, consume);
			return AddSubscriber(subscriber.Consume);
		}

		public Unsubscribe Subscribe(Action<T> consume, int interval, IActionScheduler scheduler)
		{
			var subscriber = new MostRecentIntervalSubscriber<T>(consume, interval, scheduler);
			return AddSubscriber(subscriber.Consume);
		}

		public Unsubscribe Subscribe(Action<IList<T>> consume, int interval, IActionScheduler scheduler)
		{
			var subscriber = new IntervalSubscriber<T>(consume, interval, scheduler);
			return AddSubscriber(subscriber.Consume);
		}

		public Unsubscribe Subscribe<K>(Action<IDictionary<K,T>> consume, int interval, Converter<T, K> keyConverter, IActionScheduler scheduler)
		{
			var subscriber = new UniqueIntervalSubscriber<K,T>(consume, interval, keyConverter, scheduler);
			return AddSubscriber(subscriber.Consume);
		}

		private event Action<T> _subscribers;

		private Unsubscribe AddSubscriber(Action<T> consumer)
		{
			_subscribers += consumer;

			return () => _subscribers -= consumer;
		}
	}

}