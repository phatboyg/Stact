// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Channels.Configuration
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A fluent syntax for defining subscriptions against a channel network
	/// </summary>
	public interface SubscriptionConfigurator
	{
		/// <summary>
		/// Adds a channel directly without any modification
		/// </summary>
		/// <typeparam name="TChannel">The channel type</typeparam>
		/// <param name="channel">The channel instance</param>
		ChannelSubscriptionConfigurator<TChannel> Add<TChannel>(Channel<TChannel> channel);

		/// <summary>
		/// Configures a new consumer
		/// </summary>
		/// <typeparam name="TChannel">The channel type</typeparam>
		/// <returns>A chainable method to configure additional options</returns>
		ChannelSubscriptionConfigurator<TChannel> Consume<TChannel>();
	}

	/// <summary>
	/// A fluent syntax for configuration the options of a channel subscription
	/// </summary>
	/// <typeparam name="TChannel">The channel type</typeparam>
	public interface ChannelSubscriptionConfigurator<TChannel>
	{
		/// <summary>
		/// Defines a consumer that is to consume the specified message type
		/// </summary>
		/// <typeparam name="TConsumer"></typeparam>
		/// <param name="channelAccessor"></param>
		/// <returns></returns>
		ConsumerConfigurator<TConsumer, TChannel> Using<TConsumer>(ChannelAccessor<TConsumer, TChannel> channelAccessor);

		ChannelSubscriptionConfigurator<TChannel> Using(SelectiveConsumer<TChannel> consumer);
		ChannelSubscriptionConfigurator<TChannel> Using(Consumer<TChannel> consumer);


		/// <summary>
		/// Specifies an interval at which the consumer should be called with a collection
		/// of messages received during that period.
		/// </summary>
		/// <param name="interval">The time period of each interval</param>
		/// <returns></returns>
		ChannelSubscriptionConfigurator<ICollection<TChannel>> Every(TimeSpan interval);

		/// <summary>
		/// Specifies an interval at which the consumer should be called with a dictionary
		/// of messages received during that period. Only the most recent message for each
		/// key is included in the dictionary.
		/// </summary>
		/// <typeparam name="TKey">The key type</typeparam>
		/// <param name="interval">The time period of each interval</param>
		/// <param name="keyAccessor">The key accessor</param>
		/// <returns></returns>
		ChannelSubscriptionConfigurator<IDictionary<TKey,TChannel>> Every<TKey>(TimeSpan interval, KeyAccessor<TChannel,TKey> keyAccessor);
	}
}