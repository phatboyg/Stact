// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.ForNHibernate.Channels.Configuration
{
	using System;
	using Extensions;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Channels.Configuration.Internal;


	public class NHibernateChannelProviderConfiguratorImpl<TInstance, TChannel, TKey> :
		NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey>
		where TInstance : class
	{
		readonly DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> _configurator;
		readonly KeyAccessor<TChannel, TKey> _keyAccessor;
		ChannelAccessor<TInstance, TChannel> _accessor;
		Func<InstanceChannelPolicy<TInstance, TChannel>> _instanceChannelPolicy;
		SessionProvider<TChannel> _sessionProvider;

		public NHibernateChannelProviderConfiguratorImpl(
			DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> configurator)
		{
			_configurator = configurator;

			_keyAccessor = configurator.GetDistributionKeyAccessor();

			_configurator.SetProviderFactory(GetChannelProvider);
		}

		public NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> OnChannel(
			ChannelAccessor<TInstance, TChannel> accessor)
		{
			_accessor = accessor;

			return this;
		}

		public NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> UsingSessionProvider(
			SessionProvider<TChannel> sessionProvider)
		{
			_sessionProvider = sessionProvider;

			return this;
		}

		public void SetInstanceChannelPolicyFactory(Func<InstanceChannelPolicy<TInstance, TChannel>> policyFactory)
		{
			_instanceChannelPolicy = policyFactory;
		}

		public ChannelProvider<TChannel> GetChannelProvider(ChannelConfiguratorConnection<TChannel> connection)
		{
			if (_accessor == null)
			{
				throw new ChannelConfigurationException(typeof(TChannel),
				                                        "No channel accessor was specified for NHibernate instance: "
				                                        + typeof(TInstance).ToShortTypeName());
			}
			if (_keyAccessor == null)
			{
				throw new ChannelConfigurationException(typeof(TChannel),
				                                        "No message key accessor was specified for NHibernate instance: "
				                                        + typeof(TInstance).ToShortTypeName());
			}
			if (_instanceChannelPolicy == null)
			{
				throw new ChannelConfigurationException(typeof(TChannel),
				                                        "No missing instance provider specified for NHibernate instance: "
				                                        + typeof(TInstance).ToShortTypeName());
			}
			if (_sessionProvider == null)
			{
				throw new ChannelConfigurationException(typeof(TChannel),
				                                        "No NHibernate ISession provider specified for NHibernate instance: "
				                                        + typeof(TInstance).ToShortTypeName());
			}

			FiberProvider<TKey> fiberProvider = _configurator.GetConfiguredProvider(connection);

			var channelProvider = new NHibernateInstanceChannelProvider<TInstance, TChannel, TKey>(fiberProvider,
			                                                                                       _sessionProvider,
			                                                                                       _keyAccessor,
			                                                                                       _accessor,
			                                                                                       _instanceChannelPolicy());

			return channelProvider;
		}
	}
}