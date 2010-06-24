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
	using System;
	using Extensions;
	using Reflection;


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

		public InstanceChannelConfigurator<TConsumer, TChannel> Of<TConsumer>(ChannelAccessor<TConsumer, TChannel> accessor)
		{
			var configurator = new InstanceChannelConfiguratorImpl<TConsumer, TChannel>(accessor);

			_channelFactory = configurator;

			return configurator;
		}
	}


	public class InstanceChannelConfiguratorImpl<TConsumer, TChannel> :
		InstanceChannelConfigurator<TConsumer, TChannel>,
		ChannelFactory<TChannel>
	{
		readonly ChannelAccessor<TConsumer, TChannel> _accessor;
		Func<TChannel, TConsumer> _factory = DefaultConsumerFactory;

		public InstanceChannelConfiguratorImpl(ChannelAccessor<TConsumer, TChannel> accessor)
		{
			_accessor = accessor;
		}

		public Channel<TChannel> GetChannel()
		{
			var instanceChannelProvider = new InstanceChannelProvider<TConsumer, TChannel>(_factory, _accessor);

			var instanceChannel = new InstanceChannel<TChannel>(instanceChannelProvider);

			return instanceChannel;
		}

		public void ObtainedBy(Func<TConsumer> consumerFactory)
		{
			_factory = m => consumerFactory();
		}

		public void ObtainedBy(Func<TChannel, TConsumer> consumerFactory)
		{
			_factory = consumerFactory;
		}

		static TConsumer DefaultConsumerFactory(TChannel message)
		{
			try
			{
				return FastActivator<TConsumer>.Create();
			}
			catch (Exception ex)
			{
				string errorMessage = "Failed to create consumer {0} for message {1} using default factory"
					.FormatWith(typeof(TConsumer), typeof(TChannel));

				throw new ChannelConfigurationException(errorMessage, ex);
			}
		}
	}
}