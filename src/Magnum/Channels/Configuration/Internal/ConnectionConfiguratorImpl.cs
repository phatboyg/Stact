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
	using System.Collections.Generic;
	using Extensions;


	public class ConnectionConfiguratorImpl :
		ConnectionConfigurator
	{
		readonly HashSet<Channel> _boundChannels = new HashSet<Channel>();
		readonly UntypedChannel _channel;
		readonly List<ChannelConfigurator> _configurators;

		public ConnectionConfiguratorImpl(UntypedChannel channel)
		{
			_channel = channel;
			_configurators = new List<ChannelConfigurator>();
		}

		public void AddChannel<TChannel>(Channel<TChannel> channel)
		{
			var configurator = new ChannelConnectionConfiguratorImpl<TChannel>(channel);

			_configurators.Add(configurator);
		}

		public void AddUntypedChannel(UntypedChannel channel)
		{
			var configurator = new ChannelConnectionConfiguratorImpl(channel);

			_configurators.Add(configurator);
		}

		public void RegisterChannelConfigurator(ChannelConfigurator configurator)
		{
			_configurators.Add(configurator);
		}

		public ChannelConnection Complete()
		{
			_configurators.Each(x => x.ValidateConfiguration());

			_configurators.Each(configurator =>
				{
					IEnumerable<Channel> newChannels = configurator.Configure(_channel);

					newChannels.Each(channel => _boundChannels.Add(channel));
				});

			return new ChannelConnectionImpl(_channel, _boundChannels);
		}
	}


	public class ConnectionConfiguratorImpl<T> :
		ConnectionConfigurator<T>
	{
		readonly HashSet<Channel> _boundChannels = new HashSet<Channel>();
		readonly Channel<T> _channel;
		readonly List<ChannelConfigurator<T>> _configurators;

		public ConnectionConfiguratorImpl(Channel<T> channel)
		{
			_channel = channel;
			_configurators = new List<ChannelConfigurator<T>>();
		}

		public void AddChannel<TChannel>(Channel<TChannel> channel)
		{
			var configurator = new ChannelConnectionConfiguratorImpl<T, TChannel>(channel);

			_configurators.Add(configurator);
		}

		public ConsumerChannelConfigurator<T> AddUntypedChannel(UntypedChannel channel)
		{
			return this.AddConsumer(channel.Send);
		}

		public void RegisterChannelConfigurator(ChannelConfigurator<T> configurator)
		{
			_configurators.Add(configurator);
		}

		public ChannelConnection Complete()
		{
			_configurators.Each(configurator =>
				{
					IEnumerable<Channel> newChannels = configurator.Configure(_channel);

					newChannels.Each(channel => _boundChannels.Add(channel));
				});

			return new ChannelConnectionImpl<T>(_channel, _boundChannels);
		}
	}
}