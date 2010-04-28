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
	using System.Collections.Generic;
	using Extensions;

	public class TypedSubscriptionConfigurator<T> :
		SubscriptionConfigurator
	{
		private readonly HashSet<Channel> _boundChannels = new HashSet<Channel>();
		private readonly Channel<T> _channel;
		private readonly List<TypedConfigurator<T>> _configurators = new List<TypedConfigurator<T>>();

		public TypedSubscriptionConfigurator(Channel<T> channel)
		{
			_channel = channel;
		}

		public ChannelSubscriptionConfigurator<TChannel> Add<TChannel>(Channel<TChannel> channel)
		{
			var configurator = new TypedChannelSubscriptionConfigurator<T, TChannel>(channel);

			_configurators.Add(configurator);

			return configurator;
		}

		public ChannelSubscriptionConfigurator<TChannel> Consume<TChannel>()
		{
			var configurator = new TypedChannelSubscriptionConfigurator<T, TChannel>();

			_configurators.Add(configurator);

			return configurator;
		}

		public ChannelSubscription Complete()
		{
			_configurators.Each(configurator =>
				{
					IEnumerable<Channel> addedChannels = configurator.Configure(_channel);

					addedChannels.Each(channel => _boundChannels.Add(channel));
				});

			return new TypedChannelSubscription<T>(_channel, _boundChannels);
		}
	}
}