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
namespace Magnum.Infrastructure.Channels.Configuration
{
	using System;
	using Fibers;
	using Magnum.Channels;


	/// <summary>
	/// Configures an NHibernate channel provider
	/// </summary>
	/// <typeparam name="TInstance"></typeparam>
	/// <typeparam name="TChannel"></typeparam>
	public interface NHibernateChannelProviderConfigurator<TInstance, TChannel>
		where TInstance : class
	{
		NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> IdentifiedByMessageProperty<TKey>(
			KeyAccessor<TChannel, TKey> accessor);
	}


	/// <summary>
	/// Configures an NHibernate channel provider
	/// </summary>
	/// <typeparam name="TInstance"></typeparam>
	/// <typeparam name="TChannel"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey>
		where TInstance : class
	{
		void SetMissingInstanceFactory(Func<InstanceProvider<TInstance, TChannel>> providerFactory);

		NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> OnChannel(
			ChannelAccessor<TInstance, TChannel> accessor);

		NHibernateChannelProviderConfigurator<TInstance, TChannel, TKey> UsingSessionProvider(
			SessionProvider<TChannel> sessionProvider);
	}
}