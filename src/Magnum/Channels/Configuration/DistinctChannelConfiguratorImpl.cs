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
	using Fibers;


	public class DistinctChannelConfiguratorImpl<TChannel, TKey> :
		FiberModelConfigurator<DistinctChannelConfigurator<TChannel, TKey>>,
		DistinctChannelConfigurator<TChannel, TKey>,
		ChannelFactory<ICollection<TChannel>>
	{
		readonly KeyAccessor<TChannel, TKey> _keyAccessor;
		ChannelFactory<IDictionary<TKey, TChannel>> _channelFactory;

		public DistinctChannelConfiguratorImpl(KeyAccessor<TChannel, TKey> keyAccessor)
		{
			_keyAccessor = keyAccessor;
			UseThreadPool();
		}

		public Channel<ICollection<TChannel>> GetChannel()
		{
			if (_channelFactory == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel was specified for the interval channel");

			Channel<IDictionary<TKey, TChannel>> channel = _channelFactory.GetChannel();
			Fiber fiber = _fiberFactory();

			return new DistinctChannel<TChannel, TKey>(fiber, _keyAccessor, channel);
		}

		public ChannelConnectionConfigurator<IDictionary<TKey, TChannel>> SetChannelFactory(
			ChannelFactory<IDictionary<TKey, TChannel>> channelFactory)
		{
			_channelFactory = channelFactory;

			return this;
		}
	}
}