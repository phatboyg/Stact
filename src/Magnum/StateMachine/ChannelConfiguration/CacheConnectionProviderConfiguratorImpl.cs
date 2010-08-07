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
namespace Magnum.StateMachine.ChannelConfiguration
{
	using System;
	using Channels;
	using Collections;
	using Fibers;


	public class CacheConnectionProviderConfiguratorImpl<T, TKey, TBinding> :
		CacheConnectionProviderConfigurator<T, TKey>,
		ChannelProviderFactory<T, TKey>
		where T : StateMachine<T>
	{
		readonly StateMachineConnectionConfigurator<T, TKey, TBinding> _configurator;
		Cache<TKey, T> _cache;

		public CacheConnectionProviderConfiguratorImpl(StateMachineConnectionConfigurator<T, TKey, TBinding> configurator)
		{
			_configurator = configurator;

			_configurator.SetProviderFactory(this);
		}

		public CacheConnectionProviderConfiguratorImpl(StateMachineConnectionConfigurator<T, TKey, TBinding> configurator,
		                                               Cache<TKey, T> cache)
			: this(configurator)
		{
			_cache = cache;
		}

		public ChannelProvider<TChannel> GetChannelProvider<TChannel>(ChannelAccessor<T, TChannel> channelAccessor,
		                                                              KeyAccessor<TChannel, TKey> messageKeyAccessor)
		{
			Guard.AgainstNull(channelAccessor, "channelAccessor");
			Guard.AgainstNull(messageKeyAccessor, "messageKeyAccessor");

			if (_cache == null)
			{
				Func<TKey, T> missingInstanceProvider = _configurator.GetConfiguredInstanceFactory();
				_cache = new Cache<TKey, T>(missingInstanceProvider);
			}

			Cache<TKey, T> cache = _cache;

			var instanceProvider = new DelegateChannelProvider<TChannel>(msg =>
				{
					TKey key = messageKeyAccessor(msg);

					return channelAccessor(cache[key]);
				});

			FiberProvider<TKey> fiberProvider = _configurator.GetConfiguredProvider();

			var channelProvider = new CacheChannelProvider<TChannel, TKey>(fiberProvider, instanceProvider, messageKeyAccessor);

			return channelProvider;
		}
	}
}