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

	public class UntypedConnectionConfigurator :
		ConnectionConfigurator
	{
		private readonly HashSet<Channel> _boundChannels = new HashSet<Channel>();
		private readonly UntypedChannel _channel;
		private readonly List<UntypedConfigurator> _configurators = new List<UntypedConfigurator>();

		public UntypedConnectionConfigurator(UntypedChannel channel)
		{
			_channel = channel;
		}

		public void AddChannel<TChannel>(Channel<TChannel> channel)
		{
			var configurator = new UntypedChannelConnectionConfigurator<TChannel>(channel);

			_configurators.Add(configurator);
		}

		public void AddUntypedChannel(UntypedChannel channel)
		{
			var configurator = new UntypedChannelConnectionConfigurator(channel);

			_configurators.Add(configurator);
		}

		public ChannelConnectionConfigurator<T> AddConsumerOf<T>()
		{
			var configurator = new UntypedChannelConnectionConfigurator<T>();

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

			return new UntypedChannelConnection(_channel, _boundChannels);
		}
	}
}