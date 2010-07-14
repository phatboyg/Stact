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
	public class FilterChannelConfiguratorImpl<TChannel> :
		FiberModelConfigurator<FilterChannelConfigurator<TChannel>>,
		FilterChannelConfigurator<TChannel>,
		ChannelFactory<TChannel>
	{
		readonly Filter<TChannel> _filter;
		ChannelFactory<TChannel> _channelFactory;

		public FilterChannelConfiguratorImpl(Filter<TChannel> filter)
		{
			_filter = filter;
			ExecuteOnThreadPoolFiber();
		}

		public Channel<TChannel> GetChannel()
		{
			if (_channelFactory == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel was specified for the filter output channel");

			Channel<TChannel> channel = _channelFactory.GetChannel();

			return new FilterChannel<TChannel>(_fiberFactory(), channel, _filter);
		}

		public ChannelConnectionConfigurator<TChannel> SetChannelFactory(ChannelFactory<TChannel> channelFactory)
		{
			_channelFactory = channelFactory;

			return this;
		}
	}
}