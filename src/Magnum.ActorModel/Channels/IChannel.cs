namespace Magnum.ActorModel.Channels
{
	using System;
	using System.Collections.Generic;

	public delegate void Unsubscribe();

	public interface IChannel
	{
	}

	public interface IChannel<T> :
		IChannel
	{
		/// <summary>
		/// Publish a message to the channel
		/// </summary>
		/// <param name="message"></param>
		/// <returns>True if there was a consumer for the message, otherwise false</returns>
		bool Publish(T message);

		/// <summary>
		/// Subscribes a consumer to the channel that will be called for every message that gets published
		/// </summary>
		/// <param name="actor"></param>
		/// <param name="consume"></param>
		/// <returns></returns>
		Unsubscribe Subscribe(IActor actor, Action<T> consume);

		/// <summary>
		/// Subscribes a consumer to the channel that will only receive the latest message published after each interval
		/// </summary>
		/// <param name="consume"></param>
		/// <param name="interval"></param>
		/// <param name="scheduler"></param>
		/// <returns></returns>
		Unsubscribe Subscribe(Action<T> consume, int interval, IActionScheduler scheduler);

		/// <summary>
		/// Subscribes a consumer to the channel that will called every interval with an enumeration of messages 
		/// published since the last time the consumer was called.
		/// </summary>
		/// <param name="consume"></param>
		/// <param name="interval"></param>
		/// <param name="scheduler"></param>
		/// <returns></returns>
		Unsubscribe Subscribe(Action<IList<T>> consume, int interval, IActionScheduler scheduler);

		/// <summary>
		/// Subscribes a consumer to the channel that will be called every interval with an enumeration of messages published. 
		/// Messages with the same key are eliminated from the enumeration as duplicates.
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <param name="consume"></param>
		/// <param name="keyConverter"></param>
		/// <param name="interval"></param>
		/// <param name="scheduler"></param>
		/// <returns></returns>
		Unsubscribe Subscribe<K>(Action<IDictionary<K,T>> consume, int interval, Converter<T, K> keyConverter, IActionScheduler scheduler);

		/// <summary>
		/// Remove all subscriptions to the channel
		/// </summary>
		void UnsubscribeAll();
	}
}