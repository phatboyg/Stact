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
	using System;


	public class InstanceChannelConfiguratorImpl<TChannel> :
		InstanceChannelConfigurator<TChannel>,
		ConnectionBuilderConfigurator<TChannel>
	{
		ConnectionBuilderConfigurator<TChannel> _configurator;

		public void Configure(ConnectionBuilder<TChannel> builder)
		{
			_configurator.Configure(builder);
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

		public void SetChannelConfigurator(ConnectionBuilderConfigurator<TChannel> configurator)
		{
			_configurator = configurator;
		}
	}


	public class InstanceChannelConfiguratorImpl<TInstance, TChannel> :
		FiberFactoryConfiguratorImpl<InstanceChannelConfigurator<TInstance, TChannel>>,
		InstanceChannelConfigurator<TInstance, TChannel>,
		ConnectionBuilderConfigurator<TChannel>
		where TInstance : class
	{
		Func<ConnectionBuilder<TChannel>, ChannelProvider<TChannel>> _providerFactory;

		public InstanceChannelConfiguratorImpl()
		{
			HandleOnCallingThread();
		}

		public void Configure(ConnectionBuilder<TChannel> builder)
		{
			ChannelProvider<TChannel> provider = _providerFactory(builder);

			Fiber fiber = this.GetFiberUsingConfiguredFactory(builder);

			builder.AddChannel(fiber, x => new InstanceChannel<TChannel>(x, provider));
		}

		public void ValidateConfiguration()
		{
			if (_providerFactory == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No instance provider was specified in the configuration");
		}

		public void SetProviderFactory(Func<ConnectionBuilder<TChannel>, ChannelProvider<TChannel>> providerFactory)
		{
			_providerFactory = providerFactory;
		}
	}
}