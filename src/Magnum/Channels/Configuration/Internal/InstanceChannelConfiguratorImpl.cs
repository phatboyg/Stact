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
	using Fibers;


	public class InstanceChannelConfiguratorImpl<TChannel> :
		InstanceChannelConfigurator<TChannel>,
		ChannelConfigurator<TChannel>
	{
		ChannelConfigurator<TChannel> _configurator;

		public void Configure(ChannelConfiguratorConnection<TChannel> connection)
		{
			_configurator.Configure(connection);
		}

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel configurator was setup");

			_configurator.ValidateConfiguration();
		}

		public InstanceChannelConfigurator<TInstance, TChannel> Of<TInstance>()
			where TInstance : class
		{
			var configurator = new InstanceChannelConfiguratorImpl<TInstance, TChannel>();

			SetChannelConfigurator(configurator);

			return configurator;
		}

		public void SetChannelConfigurator(ChannelConfigurator<TChannel> configurator)
		{
			_configurator = configurator;
		}
	}


	public class InstanceChannelConfiguratorImpl<TInstance, TChannel> :
		FiberModelConfigurator<InstanceChannelConfigurator<TInstance, TChannel>>,
		InstanceChannelConfigurator<TInstance, TChannel>,
		ChannelConfigurator<TChannel>
		where TInstance : class
	{
		Func<ChannelConfiguratorConnection<TChannel>, ChannelProvider<TChannel>> _providerFactory;

		public InstanceChannelConfiguratorImpl()
		{
			ExecuteOnProducerThread();
		}

		public void Configure(ChannelConfiguratorConnection<TChannel> connection)
		{
			ChannelProvider<TChannel> provider = _providerFactory(connection);

			Fiber fiber = GetConfiguredFiber(connection);

			connection.AddChannel(fiber, x => new InstanceChannel<TChannel>(x, provider));
		}

		public void ValidateConfiguration()
		{
			if (_providerFactory == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No instance provider was specified in the configuration");
		}

		public void SetProviderFactory(Func<ChannelConfiguratorConnection<TChannel>, ChannelProvider<TChannel>> providerFactory)
		{
			_providerFactory = providerFactory;
		}
	}
}