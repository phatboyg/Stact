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
namespace Stact.Configuration.Internal
{
	using System.Collections.Generic;
	using Builders;
	using Magnum.Extensions;


	public class ChannelConfiguratorImpl<TChannel> :
		ChannelConfigurator<TChannel>,
		ConnectionBuilderConfigurator,
		ConnectionBuilderConfigurator<TChannel>
	{
		readonly IList<ChannelBuilderConfigurator<TChannel>> _configurators;

		public ChannelConfiguratorImpl()
		{
			_configurators = new List<ChannelBuilderConfigurator<TChannel>>();
		}


		public void AddConfigurator(ChannelBuilderConfigurator<TChannel> configurator)
		{
			_configurators.Add(configurator);
		}

		public void ValidateConfiguration()
		{
			if (_configurators.Count == 0)
				throw new ChannelConfigurationException(typeof(TChannel), "No channels were configured");

			_configurators.Each(x => x.ValidateConfiguration());
		}

		public void Configure(ConnectionBuilder builder)
		{
			ChannelBuilder<TChannel> channelBuilder = new UntypedChannelBuilder<TChannel>(builder);

			_configurators.Each(configurator => configurator.Configure(channelBuilder));
		}

		public void Configure(ConnectionBuilder<TChannel> builder)
		{
			ChannelBuilder<TChannel> channelBuilder = new TypedChannelBuilder<TChannel>(builder);

			_configurators.Each(x => x.Configure(channelBuilder));
		}
	}
}