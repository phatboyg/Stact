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
	using Magnum.Extensions;


	public class InstanceChannelProviderConfiguratorImpl<TInstance, TChannel> :
		InstanceChannelProviderConfigurator<TInstance, TChannel>
		where TInstance : class
	{
		readonly Func<InstanceProvider<TInstance, TChannel>> _instanceProvider;
		ChannelAccessor<TInstance, TChannel> _accessor;

		public InstanceChannelProviderConfiguratorImpl(Func<InstanceProvider<TInstance, TChannel>> instanceProvider)
		{
			_instanceProvider = instanceProvider;
		}

		public InstanceChannelProviderConfigurator<TInstance, TChannel> OnChannel(
			ChannelAccessor<TInstance, TChannel> accessor)
		{
			_accessor = accessor;

			return this;
		}

		public ChannelProvider<TChannel> GetChannelProvider(ConnectionBuilder<TChannel> connection)
		{
			if (_accessor == null)
			{
				throw new ChannelConfigurationException(typeof(TChannel),
				                                        "No channel accessor was specified for instance: "
				                                        + typeof(TInstance).ToShortTypeName());
			}

			return new InstanceChannelProvider<TInstance, TChannel>(_instanceProvider(), _accessor);
		}
	}
}