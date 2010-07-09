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
namespace Magnum.Infrastructure.Channels
{
	using System;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Channels.Configuration.Internal;
	using NHibernate;
	using Reflection;


	public static class ExtensionsForNHibernateInstanceChannel
	{
		public static NHibernateChannelProviderConfigurator<TInstance, TChannel> PersistedUsingNHibernate<TInstance, TChannel>(
			this InstanceChannelConfigurator<TInstance, TChannel> configurator)
			where TInstance : class
		{
			var providerConfigurator = new NHibernateChannelProviderConfiguratorImpl<TInstance, TChannel>();

			configurator.SetProviderFactory(providerConfigurator.GetChannelProvider);

			return providerConfigurator;
		}

		public static NHibernateChannelProviderConfigurator<TInstance, TChannel> CreateMissingInstanceBy<TInstance, TChannel>(
					this NHibernateChannelProviderConfigurator<TInstance, TChannel> configurator, Func<TInstance> consumerFactory)
					where TInstance : class
		{
			return CreateMissingInstanceBy(configurator, _ => consumerFactory());
		}

		public static NHibernateChannelProviderConfigurator<TInstance, TChannel> CreateMissingInstanceBy<TInstance, TChannel>(
			this NHibernateChannelProviderConfigurator<TInstance, TChannel> configurator, Func<TChannel, TInstance> consumerFactory)
			where TInstance : class
		{
			Func<InstanceProvider<TInstance, TChannel>> instanceProvider =
				() => new DelegateInstanceProvider<TInstance, TChannel>(consumerFactory);

			configurator.SetMissingInstanceFactory(instanceProvider);

			return configurator;
		}

		public static NHibernateChannelProviderConfigurator<TInstance, TChannel> CreateMissingInstanceBy<TInstance, TChannel>(
			this NHibernateChannelProviderConfigurator<TInstance, TChannel> configurator,
			InstanceProvider<TInstance, TChannel> instanceProvider)
			where TInstance : class
		{
			configurator.SetMissingInstanceFactory(() => instanceProvider);

			return configurator;
		}

//		public static InstanceChannelConfigurator<TInstance,TChannel> StoredIn<TInstance, TChannel, TKey>(
//			this InstanceChannelConfigurator<TInstance, TChannel> configurator,
//			Func<ISessionFactory> sessionFactoryFactory,
//			KeyAccessor<TInstance, TKey> keyAccessor,
//			KeyAccessor<TChannel,TKey> messageKeyAccessor)
//			where TInstance : class
//		{
//			var missingInstanceProvider = new DelegateInstanceProvider<TInstance, TChannel>(m =>
//				{
//					var id = messageKeyAccessor(m);
//					return FastActivator<TInstance>.Create(id);
//				});
//
//			var channelProvider = new DelegateInstanceProvider<TInstance, TChannel>(m =>
//			{
//				var fiber = new SynchronousFiber();
//
//			var channel = new NHibernateInstanceChannel<TInstance, TChannel, TKey>(fiber, sessionFactoryFactory(), messageKeyAccessor,
//configurator.ChannelAccessor,
//																		  missingInstanceProvider);
//
//				return channel;
//
//		}
	}
}