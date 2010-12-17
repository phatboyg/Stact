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
	using Builders;
	using Stact.Configuration;


	/// <summary>
	/// Implements the factory that creates the instance channel backed by a 
	/// KeyedChannelProvider, which maintains an instance of the channel for each
	/// unique key value
	/// </summary>
	/// <typeparam name="TChannel"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TInstance"></typeparam>
	public class DistributedInstanceChannelConfiguratorImpl<TInstance, TChannel, TKey> :
		FiberProviderConfiguratorImpl<DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey>, TKey>,
		DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey>
		where TInstance : class
	{
		readonly KeyAccessor<TChannel, TKey> _keyAccessor;
		Func<ChannelBuilder<TChannel>, ChannelProvider<TChannel>> _providerFactory;

		public DistributedInstanceChannelConfiguratorImpl(KeyAccessor<TChannel, TKey> keyAccessor)
		{
			_keyAccessor = keyAccessor;

			HandleOnPoolFiber();
		}

		public void SetProviderFactory(
			Func<ChannelBuilder<TChannel>, ChannelProvider<TChannel>> providerFactory)
		{
			_providerFactory = providerFactory;
		}

		public FiberProvider<TKey> GetConfiguredProvider(ChannelBuilder<TChannel> builder)
		{
			FiberProvider<TKey> configuredProvider = GetConfiguredFiberProvider();
			builder.AddDisposable(configuredProvider);

			return configuredProvider;
		}

		public KeyAccessor<TChannel, TKey> GetDistributionKeyAccessor()
		{
			return _keyAccessor;
		}

		public ChannelProvider<TChannel> GetChannelProvider(ChannelBuilder<TChannel> builder)
		{
			if (_providerFactory == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No instance provider was specified in the configuration");

			var keyedProvider = new KeyedChannelProvider<TChannel, TKey>(_providerFactory(builder), _keyAccessor);

			return keyedProvider;
		}
	}
}