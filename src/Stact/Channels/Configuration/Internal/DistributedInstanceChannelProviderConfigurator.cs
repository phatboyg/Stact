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


	/// <summary>
	/// An instance configuration that is distributed by a specified key
	/// </summary>
	/// <typeparam name="TInstance"></typeparam>
	/// <typeparam name="TChannel"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> :
		FiberProviderConfigurator<DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey>, TKey>
		where TInstance : class
	{
		FiberProvider<TKey> GetConfiguredProvider(ChannelBuilder<TChannel> connection);

		KeyAccessor<TChannel, TKey> GetDistributionKeyAccessor();

		void SetProviderFactory(Func<ChannelBuilder<TChannel>, ChannelProvider<TChannel>> providerFactory);
	}
}