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


	public class InstanceChannelConfiguratorImpl<TChannel> :
		InstanceChannelConfigurator<TChannel>,
		ChannelFactory<TChannel>
	{
		ChannelFactory<TChannel> _channelFactory;

		public Channel<TChannel> GetChannel()
		{
			if (_channelFactory == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No instance provider specified");

			return _channelFactory.GetChannel();
		}

		public InstanceChannelConfigurator<TInstance, TChannel> Of<TInstance>() 
			where TInstance : class
		{
			var configurator = new InstanceChannelConfiguratorImpl<TInstance, TChannel>();

			_channelFactory = configurator;

			return configurator;
		}
	}


	public class InstanceChannelConfiguratorImpl<TInstance, TChannel> :
		InstanceChannelConfigurator<TInstance, TChannel>,
		ChannelFactory<TChannel>
		where TInstance : class
	{
		Func<ChannelProvider<TChannel>> _providerFactory;

		public Channel<TChannel> GetChannel()
		{
			if (_providerFactory == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No instance provider was specified in the configuration");

			ChannelProvider<TChannel> provider =  _providerFactory();

			var instanceChannel = new InstanceChannel<TChannel>(provider);

			return instanceChannel;
		}

		public void SetProviderFactory(Func<ChannelProvider<TChannel>> providerFactory)
		{
			_providerFactory = providerFactory;
		}
	}
}