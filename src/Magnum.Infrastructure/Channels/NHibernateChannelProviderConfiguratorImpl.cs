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
	using Extensions;
	using Magnum.Channels;


	public class NHibernateChannelProviderConfiguratorImpl<TInstance, TChannel> :
		NHibernateChannelProviderConfigurator<TInstance, TChannel>
		where TInstance : class
	{
		ChannelAccessor<TInstance, TChannel> _accessor;
		Func<TChannel, object> _keyAccessor;
		Func<InstanceProvider<TInstance, TChannel>> _missingInstanceProvider;

		public NHibernateChannelProviderConfigurator<TInstance, TChannel> OnChannel(
			ChannelAccessor<TInstance, TChannel> accessor)
		{
			_accessor = accessor;

			return this;
		}

		public NHibernateChannelProviderConfigurator<TInstance, TChannel> IdentifiedByMessageProperty(
			Func<TChannel, object> accessor)
		{
			_keyAccessor = accessor;

			return this;
		}

		public void SetMissingInstanceFactory(Func<InstanceProvider<TInstance, TChannel>> providerFactory)
		{
			_missingInstanceProvider = providerFactory;
		}

		public ChannelProvider<TChannel> GetChannelProvider()
		{
			if (_accessor == null)
			{
				throw new ChannelConfigurationException(typeof(TChannel),
				                                        "No channel accessor was specified for NHibernate instance: "
				                                        + typeof(TInstance).ToShortTypeName());
			}
			if (_keyAccessor == null)
			{
				throw new ChannelConfigurationException(typeof(TChannel),
				                                        "No message key accessor was specified for NHibernate instance: "
				                                        + typeof(TInstance).ToShortTypeName());
			}
			if (_missingInstanceProvider == null)
			{
				throw new ChannelConfigurationException(typeof(TChannel),
				                                        "No missing instance provider specified for NHibernate instance: "
				                                        + typeof(TInstance).ToShortTypeName());
			}

			return new InstanceChannelProvider<TInstance, TChannel>(_missingInstanceProvider(), _accessor);
		}
	}
}