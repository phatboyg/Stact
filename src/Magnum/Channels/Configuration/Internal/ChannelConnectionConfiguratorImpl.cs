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


	public class ChannelConnectionConfiguratorImpl :
		ChannelConnectionConfigurator,
		ChannelConfigurator
	{
		readonly UntypedChannel _channel;

		public ChannelConnectionConfiguratorImpl(UntypedChannel channel)
		{
			Guard.AgainstNull(channel);

			_channel = channel;
		}

		public void Configure(CreateChannelConnection connection, UntypedChannel channel)
		{
			new ConnectChannelVisitor(_channel).ConnectTo(channel);

			connection.AddChannel(_channel);
		}

		public void ValidateConfiguration()
		{
		}
	}


	public class ChannelConnectionConfiguratorImpl<TChannel> :
		ChannelConnectionConfigurator<TChannel>,
		ChannelConfigurator,
		ChannelConfigurator<TChannel>
	{
		Func<Channel<TChannel>> _channelFactory;

		public ChannelConnectionConfiguratorImpl()
		{
		}

		public ChannelConnectionConfiguratorImpl(Channel<TChannel> channel)
		{
			Guard.AgainstNull(channel);

			_channelFactory = () => channel;
		}

		public void Configure(CreateChannelConnection connection, UntypedChannel channel)
		{
			Channel<TChannel> newChannel = _channelFactory();

			new ConnectChannelVisitor<TChannel>(newChannel).ConnectTo(channel);

			connection.AddChannel(newChannel);
		}

		public void ValidateConfiguration()
		{
			if (_channelFactory == null)
				throw new ChannelConfigurationException("No channel factory specified for channel: " + typeof(TChannel).Name);
		}

		public void Configure(CreateChannelConnection connection, Channel<TChannel> channel)
		{
			Channel<TChannel> newChannel = _channelFactory();

			new ConnectChannelVisitor<TChannel>(newChannel).ConnectTo(channel);

			connection.AddChannel(newChannel);
		}

		public ChannelConnectionConfigurator<TChannel> SetChannelFactory(ChannelFactory<TChannel> channelFactory)
		{
			_channelFactory = channelFactory.GetChannel;

			return this;
		}
	}


	public class ChannelConnectionConfiguratorImpl<T, TChannel> :
		ChannelConfigurator<T>
	{
		readonly Channel<TChannel> _channel;

		public ChannelConnectionConfiguratorImpl(Channel<TChannel> channel)
		{
			_channel = channel;
		}

		public void Configure(CreateChannelConnection connection, Channel<T> channel)
		{
			new ConnectChannelVisitor<TChannel>(_channel).ConnectTo(channel);

			connection.AddChannel(_channel);
		}

		public void ValidateConfiguration()
		{
			if (_channel == null)
				throw new ChannelConfigurationException("A null channel was specified");
		}
	}
}