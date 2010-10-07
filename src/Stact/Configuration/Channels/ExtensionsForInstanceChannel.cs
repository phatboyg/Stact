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
namespace Stact
{
	using System;
	using Configuration;
	using Configuration.Internal;


	public static class ExtensionsForInstanceChannel
	{
		public static InstanceChannelConfigurator<TChannel> UsingInstance<TChannel>(
			this ChannelConnectionConfigurator<TChannel> configurator)
		{
			var instanceConfigurator = new InstanceChannelConfiguratorImpl<TChannel>();

			configurator.SetChannelConfigurator(instanceConfigurator);

			return instanceConfigurator;
		}

		public static InstanceChannelProviderConfigurator<TInstance, TChannel> ObtainedBy<TInstance, TChannel>(
			this InstanceChannelConfigurator<TInstance, TChannel> configurator, Func<TInstance> consumerFactory)
			where TInstance : class
		{
			return ObtainedBy(configurator, _ => consumerFactory());
		}

		public static InstanceChannelProviderConfigurator<TInstance, TChannel> ObtainedBy<TInstance, TChannel>(
			this InstanceChannelConfigurator<TInstance, TChannel> configurator, Func<TChannel, TInstance> consumerFactory)
			where TInstance : class
		{
			Func<InstanceProvider<TInstance, TChannel>> instanceProvider =
				() => new DelegateInstanceProvider<TInstance, TChannel>(consumerFactory);

			var providerConfigurator = new InstanceChannelProviderConfiguratorImpl<TInstance, TChannel>(instanceProvider);

			configurator.SetProviderFactory(providerConfigurator.GetChannelProvider);

			return providerConfigurator;
		}

		public static InstanceChannelProviderConfigurator<TInstance, TChannel> ObtainedBy<TInstance, TChannel>(
			this InstanceChannelConfigurator<TInstance, TChannel> configurator,
			InstanceProvider<TInstance, TChannel> instanceProvider)
			where TInstance : class
		{
			var providerConfigurator = new InstanceChannelProviderConfiguratorImpl<TInstance, TChannel>(() => instanceProvider);

			configurator.SetProviderFactory(providerConfigurator.GetChannelProvider);

			return providerConfigurator;
		}
	}
}