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
	using Fibers;


	public class LastChannelConfiguratorImpl<TChannel> :
		FiberModelConfigurator<LastChannelConfigurator<TChannel>>,
		LastChannelConfigurator<TChannel>,
		ChannelFactory<ICollection<TChannel>>
	{
		ChannelFactory<TChannel> _channelFactory;

		public LastChannelConfiguratorImpl()
		{
			ExecuteOnProducerThread();
		}

		public Channel<ICollection<TChannel>> GetChannel()
		{
			if (_channelFactory == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel was specified for the interval channel");

			Channel<TChannel> channel = _channelFactory.GetChannel();
			Fiber fiber = _fiberFactory();

			return new LastChannel<TChannel>(fiber, channel);
		}

		public ChannelConnectionConfigurator<TChannel> SetChannelFactory(
			ChannelFactory<TChannel> channelFactory)
		{
			_channelFactory = channelFactory;

			return this;
		}
	}
}