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
namespace Magnum.Channels.Configuration.Internal
{
	using Fibers;
	using Fibers.Configuration;


	public class ConsumerChannelConfiguratorImpl<TChannel> :
		FiberConfiguratorImpl<ConsumerChannelConfigurator<TChannel>>,
		ConsumerChannelConfigurator<TChannel>,
		ChannelConfigurator,
		ChannelConfigurator<TChannel>
	{
		readonly Consumer<TChannel> _consumer;

		public ConsumerChannelConfiguratorImpl(Consumer<TChannel> consumer)
		{
			_consumer = consumer;
		}

		public void ValidateConfiguration()
		{
			if (_consumer == null)
				throw new ChannelConfigurationException(typeof(TChannel), "Consumer cannot be null");
		}

		public void Configure(ChannelConfiguratorConnection connection)
		{
			Fiber fiber = GetConfiguredFiber(connection);

			connection.AddChannel(fiber, x => new ConsumerChannel<TChannel>(x, _consumer));
		}

		public void Configure(ChannelConfiguratorConnection<TChannel> connection)
		{
			Fiber fiber = GetConfiguredFiber(connection);

			connection.AddChannel(fiber, x => new ConsumerChannel<TChannel>(x, _consumer));
		}
	}
}