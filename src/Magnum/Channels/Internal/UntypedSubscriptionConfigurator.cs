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
namespace Magnum.Channels.Internal
{
	using System.Collections.Generic;
	using Extensions;

	public class UntypedSubscriptionConfigurator :
		SubscriptionConfigurator
	{
		private readonly HashSet<Channel> _boundChannels = new HashSet<Channel>();
		private readonly UntypedChannel _channel;
		private readonly List<UntypedConfigurator> _configurators = new List<UntypedConfigurator>();

		public UntypedSubscriptionConfigurator(UntypedChannel channel)
		{
			_channel = channel;
		}

		public ChannelSubscriptionConfigurator<TChannel> Add<TChannel>(Channel<TChannel> channel)
		{
			var configurator = new UntypedChannelSubscriptionConfigurator<TChannel>(channel);

			_configurators.Add(configurator);

			return configurator;
		}

		public ChannelSubscriptionConfigurator<T> Consume<T>()
		{
			var configurator = new UntypedChannelSubscriptionConfigurator<T>();

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

			return new UntypedChannelSubscription(_channel, _boundChannels);
		}
	}
}