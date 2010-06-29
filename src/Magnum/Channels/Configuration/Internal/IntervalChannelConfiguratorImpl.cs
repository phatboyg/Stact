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
	using System;
	using System.Collections.Generic;


	public class IntervalChannelConfiguratorImpl<TChannel> :
		IntervalModelConfigurator<IntervalChannelConfigurator<TChannel>>,
		IntervalChannelConfigurator<TChannel>,
		ChannelFactory<TChannel>
	{
		ChannelFactory<ICollection<TChannel>> _channelFactory;

		public IntervalChannelConfiguratorImpl(TimeSpan interval)
		{
			_interval = interval;

			UsePrivateScheduler();
			UseThreadPool();
		}

		public Channel<TChannel> GetChannel()
		{
			if (_channelFactory == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel was specified for the interval channel");

			Channel<ICollection<TChannel>> channel = _channelFactory.GetChannel();

			return new IntervalChannel<TChannel>(_fiberFactory(), _schedulerFactory(), _interval, channel);
		}

		public ChannelConnectionConfigurator<ICollection<TChannel>> SetChannelFactory(
			ChannelFactory<ICollection<TChannel>> channelFactory)
		{
			_channelFactory = channelFactory;

			return this;
		}
	}
}