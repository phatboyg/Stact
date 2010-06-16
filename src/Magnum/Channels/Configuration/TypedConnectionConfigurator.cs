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
	using Extensions;
	using Fibers;

	public class TypedConnectionConfigurator<T> :
		ConnectionConfigurator
	{
		private readonly HashSet<Channel> _boundChannels = new HashSet<Channel>();
		private readonly Channel<T> _channel;
		private readonly List<TypedConfigurator<T>> _configurators = new List<TypedConfigurator<T>>();

		public TypedConnectionConfigurator(Channel<T> channel)
		{
			_channel = channel;
		}

		public ChannelConnectionConfigurator<TChannel> Add<TChannel>(Channel<TChannel> channel)
		{
			var configurator = new TypedChannelConnectionConfigurator<T, TChannel>(channel);

			_configurators.Add(configurator);

			return configurator;
		}

		public void Add(UntypedChannel channel)
		{
			var consumerChannel = new ConsumerChannel<T>(new SynchronousFiber(), channel.Send);

			var configurator =new TypedChannelConnectionConfigurator<T, T>(consumerChannel);

			_configurators.Add(configurator);
		}

		public ChannelConnectionConfigurator<TChannel> Consume<TChannel>()
		{
			var configurator = new TypedChannelConnectionConfigurator<T, TChannel>();

			_configurators.Add(configurator);

			return configurator;
		}

		public ChannelConnection Complete()
		{
			_configurators.Each(configurator =>
				{
					IEnumerable<Channel> addedChannels = configurator.Configure(_channel);

					addedChannels.Each(channel => _boundChannels.Add(channel));
				});

			return new TypedChannelConnection<T>(_channel, _boundChannels);
		}
	}
}