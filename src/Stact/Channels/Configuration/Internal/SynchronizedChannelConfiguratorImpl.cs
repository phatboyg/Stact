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
	using System.Collections.Generic;
	using System.Threading;
	using Builders;
	using Magnum.Extensions;


	public class SynchronizedChannelConfiguratorImpl<TChannel> :
		SynchronizedChannelConfigurator<TChannel>,
		ChannelBuilderConfigurator<TChannel>
	{
		readonly IList<ChannelBuilderConfigurator<TChannel>> _configurators;
		readonly SynchronizationContext _synchronizationContext;
		object _state;

		public SynchronizedChannelConfiguratorImpl()
			: this(SynchronizationContext.Current)
		{
		}

		public SynchronizedChannelConfiguratorImpl(SynchronizationContext synchronizationContext)
		{
			_synchronizationContext = synchronizationContext;

			_configurators = new List<ChannelBuilderConfigurator<TChannel>>();
		}

		public void ValidateConfiguration()
		{
			_configurators.Each(x => x.ValidateConfiguration());
		}

		public void Configure(ChannelBuilder<TChannel> builder)
		{
			ChannelBuilder<TChannel> syncBuilder = builder;
			if (_synchronizationContext != null)
				syncBuilder = new SynchronizedChannelBuilder<TChannel>(builder, _synchronizationContext, _state);

			_configurators.Each(x => x.Configure(syncBuilder));
		}

		public SynchronizedChannelConfigurator<TChannel> WithState(object state)
		{
			_state = state;

			return this;
		}

		public void AddConfigurator(ChannelBuilderConfigurator<TChannel> configurator)
		{
			_configurators.Add(configurator);
		}
	}
}