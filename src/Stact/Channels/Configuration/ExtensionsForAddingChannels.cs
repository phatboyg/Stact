// Copyright 2010 Chris Patterson
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
namespace Stact
{
	using Configuration;
	using Configuration.Builders;
	using Configuration.Internal;


	public static class ExtensionsForAddingChannels
	{
		public static ConnectionConfigurator AddChannel<TChannel>(this ConnectionConfigurator configurator,
		                                                          Channel<TChannel> channel)
		{
			var channelConfigurator = new TypedChannelConfigurator<TChannel>(channel);

			configurator.AddConfigurator(channelConfigurator);

			return configurator;
		}

		public static ConnectionConfigurator<T> AddChannel<T, TChannel>(this ConnectionConfigurator<T> configurator,
		                                                                Channel<TChannel> channel)
		{
			var channelConfigurator = new TypedChannelConfigurator<T, TChannel>(channel);

			configurator.AddConfigurator(channelConfigurator);

			return configurator;
		}

		public static ConnectionConfigurator AddChannel(this ConnectionConfigurator configurator, UntypedChannel channel)
		{
			var channelConfigurator = new UntypedChannelConfigurator(channel);

			configurator.AddConfigurator(channelConfigurator);

			return configurator;
		}

		public static ConsumerConfigurator<T> AddChannel<T>(this ConnectionConfigurator<T> configurator,
		                                                      UntypedChannel channel)
		{

			var channelConfigurator = new ChannelConfiguratorImpl<T>();
			configurator.AddConfigurator(channelConfigurator);

			var consumerConfigurator = new ConsumerConfiguratorImpl<T>(channel.Send);
			channelConfigurator.AddConfigurator(consumerConfigurator);

			return consumerConfigurator;
		}
	}
}