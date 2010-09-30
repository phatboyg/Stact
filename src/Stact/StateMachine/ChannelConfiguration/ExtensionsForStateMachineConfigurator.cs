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
	using Magnum.Collections;
	using Magnum.StateMachine;
	using StateMachine.ChannelConfiguration;


	public static class ExtensionsForStateMachineConfigurator
	{
		public static StateMachineConnectionConfigurator<T, TKey, TBinding> CreateNewInstanceBy<T, TKey, TBinding>(
			this StateMachineConnectionConfigurator<T, TKey, TBinding> configurator, Func<TKey, T> consumerFactory)
			where T : StateMachine<T>
			where TBinding : StateMachineBinding<T, TKey>
		{
			configurator.SetNewInstanceFactory(consumerFactory);

			return configurator;
		}

		public static CacheConnectionProviderConfigurator<T, TKey> PersistInMemory<T, TKey, TBinding>(
			this StateMachineConnectionConfigurator<T, TKey, TBinding> configurator)
			where T : StateMachine<T>
		{
			var providerConfigurator = new CacheConnectionProviderConfiguratorImpl<T, TKey, TBinding>(configurator);

			return providerConfigurator;
		}

		public static CacheConnectionProviderConfigurator<T, TKey> PersistInMemoryUsing<T, TKey, TBinding>(
			this StateMachineConnectionConfigurator<T, TKey, TBinding> configurator, Cache<TKey, T> cache)
			where T : StateMachine<T>
		{
			var providerConfigurator = new CacheConnectionProviderConfiguratorImpl<T, TKey, TBinding>(configurator, cache);

			return providerConfigurator;
		}
	}
}