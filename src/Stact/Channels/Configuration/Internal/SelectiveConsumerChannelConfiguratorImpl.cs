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
namespace Stact.Channels.Configuration.Internal
{
	using Fibers;
	using Stact.Configuration;


	public class SelectiveConsumerChannelConfiguratorImpl<TChannel> :
		FiberFactoryConfiguratorImpl<ConsumerChannelConfigurator<TChannel>>,
		ConsumerChannelConfigurator<TChannel>,
		ChannelConfigurator<TChannel>,
		ChannelConfigurator
	{
		readonly SelectiveConsumer<TChannel> _consumer;

		public SelectiveConsumerChannelConfiguratorImpl(SelectiveConsumer<TChannel> consumer)
		{
			_consumer = consumer;
		}

		public void Configure(ChannelConfiguratorConnection connection)
		{
			Fiber fiber = GetFiberUsingConfiguredFactory(connection);

			connection.AddChannel(fiber, x => new SelectiveConsumerChannel<TChannel>(x, _consumer));
		}

		public void Configure(ChannelConfiguratorConnection<TChannel> connection)
		{
			Fiber fiber = GetFiberUsingConfiguredFactory(connection);

			connection.AddChannel(fiber, x => new SelectiveConsumerChannel<TChannel>(x, _consumer));
		}

		public void ValidateConfiguration()
		{
			if (_consumer == null)
				throw new ChannelConfigurationException(typeof(TChannel), "Consumer cannot be null");
		}
	}
}