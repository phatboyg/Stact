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
namespace Stact.ForNHibernate.Channels.Configuration
{
	using System;
	using Magnum.Extensions;
	using Fibers;
	using Magnum.StateMachine;
	using Stact.Channels;
	using Stact.StateMachine;
	using Stact.StateMachine.ChannelConfiguration;
	using NHibernate;


	public class NHibernateConnectionProviderConfiguratorImpl<T, TKey, TBinding> :
		NHibernateConnectionProviderConfigurator<T, TKey>,
		ChannelProviderFactory<T, TKey>
		where T : StateMachine<T>
	{
		readonly StateMachineConnectionConfigurator<T, TKey, TBinding> _configurator;
		LateBoundSessionProvider _sessionProvider;

		public NHibernateConnectionProviderConfiguratorImpl(StateMachineConnectionConfigurator<T, TKey, TBinding> configurator)
		{
			_configurator = configurator;

			_configurator.SetProviderFactory(this);
		}

		public ChannelProvider<TChannel> GetChannelProvider<TChannel>(ChannelAccessor<T, TChannel> channelAccessor,
		                                                              KeyAccessor<TChannel, TKey> messageKeyAccessor,
		                                                              InstanceChannelPolicy<T, TChannel> channelPolicy)
		{
			Magnum.Guard.AgainstNull(channelAccessor, "channelAccessor");
			Magnum.Guard.AgainstNull(messageKeyAccessor, "messageKeyAccessor");
			Magnum.Guard.AgainstNull(channelPolicy, "channelPolicy");

			FiberProvider<TKey> fiberProvider = _configurator.GetConfiguredProvider();

			if (_sessionProvider == null)
			{
				throw new ChannelConfigurationException(typeof(TChannel),
				                                        "No NHibernate ISession provider specified for NHibernate instance: "
				                                        + typeof(T).ToShortTypeName());
			}

			SessionProvider<TChannel> sessionProvider = m => _sessionProvider.GetSession(m);

			var channelProvider = new NHibernateInstanceChannelProvider<T, TChannel, TKey>(fiberProvider,
			                                                                               sessionProvider,
			                                                                               messageKeyAccessor,
			                                                                               channelAccessor,
			                                                                               channelPolicy);

			return channelProvider;
		}

		public NHibernateConnectionProviderConfigurator<T, TKey> UseSessionProvider(LateBoundSessionProvider sessionProvider)
		{
			_sessionProvider = sessionProvider;

			return this;
		}

		public NHibernateConnectionProviderConfigurator<T, TKey> UseSessionProvider(Func<ISession> sessionProvider)
		{
			_sessionProvider = new DelegateSessionProvider(sessionProvider);

			return this;
		}
	}
}