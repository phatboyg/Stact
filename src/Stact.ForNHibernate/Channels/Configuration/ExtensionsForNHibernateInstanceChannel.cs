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
namespace Stact.Channels
{
	using System;
	using Configuration.Internal;
	using ForNHibernate.Channels.Configuration;
	using Internal;


	public static class ExtensionsForNHibernateInstanceChannel
	{
		public static NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> PersistUsingNHibernate
			<TInstance, TChannel, TKey>(
			this DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> configurator)
			where TInstance : class
		{
			var providerConfigurator = new NHibernateChannelProviderConfiguratorImpl<TInstance, TChannel, TKey>(configurator);

			return providerConfigurator;
		}

		public static NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> CreateNewInstanceBy
			<TInstance, TChannel, TKey>(
			this NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> configurator, Func<TInstance> consumerFactory)
			where TInstance : class
		{
			return CreateNewInstanceBy(configurator, _ => consumerFactory());
		}

		public static NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> CreateNewInstanceBy
			<TInstance, TChannel, TKey>(
			this NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> configurator,
			Func<TChannel, TInstance> consumerFactory)
			where TInstance : class
		{
			Func<InstanceChannelPolicy<TInstance, TChannel>> policyFactory =
				() =>
				new CreateOrUseExistingInstanceChannelPolicy<TInstance, TChannel>(
					new DelegateInstanceProvider<TInstance, TChannel>(consumerFactory));

			configurator.SetInstanceChannelPolicyFactory(policyFactory);

			return configurator;
		}

		public static NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> CreateNewInstanceBy
			<TInstance, TChannel, TKey>(
			this NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> configurator,
			InstanceProvider<TInstance, TChannel> instanceProvider)
			where TInstance : class
		{
			Func<InstanceChannelPolicy<TInstance, TChannel>> policyFactory =
				() =>
				new CreateOrUseExistingInstanceChannelPolicy<TInstance, TChannel>(instanceProvider);

			configurator.SetInstanceChannelPolicyFactory(policyFactory);

			return configurator;
		}

		public static NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> UseOnlyExistingInstance
			<TInstance, TChannel, TKey>(
			this NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> configurator)
			where TInstance : class
		{
			Func<InstanceChannelPolicy<TInstance, TChannel>> policyFactory =
				() =>
				new ExistingInstanceChannelPolicy<TInstance, TChannel>();

			configurator.SetInstanceChannelPolicyFactory(policyFactory);

			return configurator;
		}
	}
}