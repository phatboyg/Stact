namespace Magnum.ActorModel.Channels
{
	using System;

	public delegate void Unsubscribe();

	public interface Channel
	{
		/// <summary>
		/// Remove all subscriptions to the channel
		/// </summary>
		void UnsubscribeAll();
	}

	public interface Channel<T> :
		Channel
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
		/// <returns></returns>
		Unsubscribe Subscribe(Action<T> consumer);
	}
}