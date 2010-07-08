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
	using System;


	public class ConsumerChannelConfiguratorImpl<TChannel> :
		ChannelModelConfigurator<ConsumerChannelConfigurator<TChannel>, TChannel>,
		ConsumerChannelConfigurator<TChannel>,
		ChannelFactory<TChannel>,
		ChannelConfigurator<TChannel>
	{
		readonly Consumer<TChannel> _consumer;

		public ConsumerChannelConfiguratorImpl(Consumer<TChannel> consumer)
		{
			_consumer = consumer;
		}

		public void Configure(CreateChannelConnection connection, Channel<TChannel> channel)
		{
			throw new NotImplementedException();
		}

		public void ValidateConfiguration()
		{
			throw new NotImplementedException();
		}

		public Channel<TChannel> GetChannel()
		{
			return CreateChannel(() => new ConsumerChannel<TChannel>(_fiberFactory(), _consumer));
		}
	}
}