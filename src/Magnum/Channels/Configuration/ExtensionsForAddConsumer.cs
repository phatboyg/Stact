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


	public static class ExtensionsForAddConsumer
	{
		/// <summary>
		/// Configures a new consumer
		/// </summary>
		/// <typeparam name="TChannel">The channel type</typeparam>
		/// <returns>A chainable method to configure additional options</returns>
		public static ChannelConnectionConfigurator<TChannel> AddConsumerOf<TChannel>(
			this ConnectionConfigurator connectionConfigurator)
		{
			var configurator = new ChannelConnectionConfiguratorImpl<TChannel>();

			connectionConfigurator.RegisterChannelConfigurator(configurator);

			return configurator;
		}

		public static ConsumerChannelConfigurator<TChannel> AddConsumer<TChannel>(
			this ConnectionConfigurator<TChannel> connectionConfigurator,
			Consumer<TChannel> consumer)
		{
			var channelConfigurator = new ChannelConnectionConfiguratorImpl<TChannel>();

			connectionConfigurator.RegisterChannelConfigurator(channelConfigurator);

			var configurator = new ConsumerChannelConfiguratorImpl<TChannel>(consumer);

			channelConfigurator.SetChannelFactory(configurator);

			return configurator;
		}
	}
}