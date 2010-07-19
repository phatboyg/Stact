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
namespace Magnum.Channels
{
	using Configuration;
	using Configuration.Internal;


	public static class ExtensionsForSelectiveConsumerChannel
	{
		/// <summary>
		/// Add a consumer to the channel using the message type defined for the channel
		/// </summary>
		/// <typeparam name="TChannel">The channel type, specifies the type of message sent by the channel</typeparam>
		/// <param name="connectionConfigurator">The connection configurator</param>
		/// <param name="consumer">The consumer to add to the channel</param>
		/// <returns>A consumer configurator to customize the consumer settings</returns>
		public static ConsumerChannelConfigurator<TChannel> AddSelectiveConsumer<TChannel>(
			this ConnectionConfigurator<TChannel> connectionConfigurator,
			SelectiveConsumer<TChannel> consumer)
		{
			var configurator = new SelectiveConsumerChannelConfiguratorImpl<TChannel>(consumer);

			connectionConfigurator.RegisterChannelConfigurator(configurator);

			return configurator;
		}

		/// <summary>
		/// Consumes the message on a SelectiveConsumerChannel given the specified delegate
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="selectiveConsumer"></param>
		/// <returns></returns>
		public static ConsumerChannelConfigurator<TChannel> UsingSelectiveConsumer<TChannel>(
			this ChannelConnectionConfigurator<TChannel> configurator, SelectiveConsumer<TChannel> selectiveConsumer)
		{
			var consumerConfigurator = new SelectiveConsumerChannelConfiguratorImpl<TChannel>(selectiveConsumer);

			configurator.SetChannelConfigurator(consumerConfigurator);

			return consumerConfigurator;
		}
	}
}