namespace Magnum.Channels.Configuration
{
	using System;
	using System.Collections.Generic;


	public interface ChannelConnectionConfigurator
	{
		// TODO
		// ChannelConnectionConfigurator OnThreadPool();
	}

	/// <summary>
	/// A fluent syntax for configuration the options of a channel subscription
	/// </summary>
	/// <typeparam name="TChannel">The channel type</typeparam>
	public interface ChannelConnectionConfigurator<TChannel>
	{
		/// <summary>
		/// Defines a consumer that is to consume the specified message type
		/// </summary>
		/// <typeparam name="TConsumer"></typeparam>
		/// <param name="channelAccessor"></param>
		/// <returns></returns>
		ConsumerConfigurator<TConsumer, TChannel> UsingInstance<TConsumer>(ChannelAccessor<TConsumer, TChannel> channelAccessor);

		/// <summary>
		/// Consumes the message on a ConsumerChannel, given the specified delegate
		/// </summary>
		/// <param name="consumer"></param>
		/// <returns></returns>
		ChannelConnectionConfigurator<TChannel> UsingConsumer(Consumer<TChannel> consumer);

		/// <summary>
		/// Consumes the message on a SelectiveConsumerChannel given the specified delegate
		/// </summary>
		/// <param name="consumer"></param>
		/// <returns></returns>
		ChannelConnectionConfigurator<TChannel> UsingSelectiveConsumer(SelectiveConsumer<TChannel> consumer);


		/// <summary>
		/// Specifies an interval at which the consumer should be called with a collection
		/// of messages received during that period.
		/// </summary>
		/// <param name="interval">The time period of each interval</param>
		/// <returns></returns>
		ChannelConnectionConfigurator<ICollection<TChannel>> Every(TimeSpan interval);

		/// <summary>
		/// Specifies an interval at which the consumer should be called with a dictionary
		/// of messages received during that period. Only the most recent message for each
		/// key is included in the dictionary.
		/// </summary>
		/// <typeparam name="TKey">The key type</typeparam>
		/// <param name="interval">The time period of each interval</param>
		/// <param name="keyAccessor">The key accessor</param>
		/// <returns></returns>
		ChannelConnectionConfigurator<IDictionary<TKey,TChannel>> Every<TKey>(TimeSpan interval, KeyAccessor<TChannel,TKey> keyAccessor);
	}
}