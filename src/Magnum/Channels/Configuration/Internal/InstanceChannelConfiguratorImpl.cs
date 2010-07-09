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

		public InstanceChannelConfigurator<TInstance, TChannel> Of<TInstance>(ChannelAccessor<TInstance, TChannel> accessor) 
			where TInstance : class
		{
			var configurator = new InstanceChannelConfiguratorImpl<TInstance, TChannel>(accessor);

			_channelFactory = configurator;

			return configurator;
		}
	}


	public class InstanceChannelConfiguratorImpl<TInstance, TChannel> :
		InstanceChannelConfigurator<TInstance, TChannel>,
		ChannelFactory<TChannel>
		where TInstance : class
	{
		readonly ChannelAccessor<TInstance, TChannel> _accessor;
		Func<ChannelProvider<TChannel>> _providerFactory;

		public InstanceChannelConfiguratorImpl(ChannelAccessor<TInstance, TChannel> accessor)
		{
			_accessor = accessor;
		}

		public Channel<TChannel> GetChannel()
		{
			ChannelProvider<TChannel> provider = _providerFactory == null ? GetDefaultProvider() : _providerFactory();

			var instanceChannel = new InstanceChannel<TChannel>(provider);

			return instanceChannel;
		}

		public void ObtainedBy(Func<TInstance> consumerFactory)
		{
			_providerFactory = () =>
				{
					var instanceProvider = new DelegateInstanceProvider<TInstance, TChannel>(_ => consumerFactory());

					return new InstanceChannelProvider<TInstance, TChannel>(instanceProvider, _accessor);
				};
		}

		public void ObtainedBy(Func<TChannel, TInstance> consumerFactory)
		{
			_providerFactory = () =>
				{
					var instanceProvider = new DelegateInstanceProvider<TInstance, TChannel>(consumerFactory);

					return new InstanceChannelProvider<TInstance, TChannel>(instanceProvider, _accessor);
				};
		}

		public ChannelAccessor<TInstance, TChannel> ChannelAccessor
		{
			get { return _accessor; }
		}

		public void SetProviderFactory(Func<ChannelProvider<TChannel>> providerFactory)
		{
			_providerFactory = providerFactory;
		}

		ChannelProvider<TChannel> GetDefaultProvider()
		{
			var instanceProvider = new FastActivatorInstanceProvider<TInstance, TChannel>();

			return new InstanceChannelProvider<TInstance, TChannel>(instanceProvider, ChannelAccessor);
		}
	}
}