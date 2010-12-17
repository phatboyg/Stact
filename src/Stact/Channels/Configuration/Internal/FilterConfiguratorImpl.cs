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


	public class FilterConfiguratorImpl<TChannel> :
		FilterConfigurator<TChannel>,
		ChannelBuilderConfigurator<TChannel>
	{
		readonly IList<ChannelBuilderConfigurator<TChannel>> _configurators;
		readonly Filter<TChannel> _filter;

		public FilterConfiguratorImpl(Filter<TChannel> filter)
		{
			_filter = filter;
			_configurators = new List<ChannelBuilderConfigurator<TChannel>>();
		}

		public void ValidateConfiguration()
		{
			if (_filter == null)
				throw new ChannelConfigurationException(typeof(TChannel), "Filter delegate was null");

			if (_configurators.Count == 0)
				throw new ChannelConfigurationException(typeof(TChannel), "No channels were configured");

			_configurators.Each(x => x.ValidateConfiguration());
		}

		public void Configure(ChannelBuilder<TChannel> builder)
		{
			var filterBuilder = new FilterChannelBuilder<TChannel>(builder, _filter);

			_configurators.Each(configurator => configurator.Configure(filterBuilder));
		}


		public void AddConfigurator(ChannelBuilderConfigurator<TChannel> configurator)
		{
			_configurators.Add(configurator);
		}
	}
}