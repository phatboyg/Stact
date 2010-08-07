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
namespace Magnum.Channels
{
	using System;
	using Configuration.Internal;
	using Infrastructure.Channels.Configuration;


	public static class ExtensionsForNHibernateInstanceChannel
	{
		public static NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> PersistedUsingNHibernate
			<TInstance, TChannel, TKey>(
			this DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> configurator)
			where TInstance : class
		{
			var providerConfigurator = new NHibernateChannelProviderConfiguratorImpl<TInstance, TChannel, TKey>(configurator);

			return providerConfigurator;
		}

		public static NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> CreateMissingInstanceBy
			<TInstance, TChannel, TKey>(
			this NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> configurator, Func<TInstance> consumerFactory)
			where TInstance : class
		{
			return CreateMissingInstanceBy(configurator, _ => consumerFactory());
		}

		public static NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> CreateMissingInstanceBy
			<TInstance, TChannel, TKey>(
			this NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> configurator,
			Func<TChannel, TInstance> consumerFactory)
			where TInstance : class
		{
			Func<InstanceProvider<TInstance, TChannel>> instanceProvider =
				() => new DelegateInstanceProvider<TInstance, TChannel>(consumerFactory);

			configurator.SetMissingInstanceFactory(instanceProvider);

			return configurator;
		}

		public static NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> CreateMissingInstanceBy
			<TInstance, TChannel, TKey>(
			this NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> configurator,
			InstanceProvider<TInstance, TChannel> instanceProvider)
			where TInstance : class
		{
			configurator.SetMissingInstanceFactory(() => instanceProvider);

			return configurator;
		}
	}
}