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
	using System.Collections.Generic;


	public class DistinctChannelConfiguratorImpl<TChannel, TKey> :
		FiberFactoryConfiguratorImpl<DistinctChannelConfigurator<TChannel, TKey>>,
		DistinctChannelConfigurator<TChannel, TKey>,
		ConnectionBuilderConfigurator<ICollection<TChannel>>
	{
		readonly KeyAccessor<TChannel, TKey> _keyAccessor;
		ConnectionBuilderConfigurator<IDictionary<TKey, TChannel>> _configurator;

		public DistinctChannelConfiguratorImpl(KeyAccessor<TChannel, TKey> keyAccessor)
		{
			_keyAccessor = keyAccessor;

			HandleOnPoolFiber();
		}

		public void Configure(ConnectionBuilder<ICollection<TChannel>> builder)
		{
			Fiber fiber = this.GetFiberUsingConfiguredFactory(builder);

			_configurator.Configure(new DistinctConnectionBuilderDecorator(builder, fiber, _keyAccessor));
		}

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel configurator was setup");

			_configurator.ValidateConfiguration();
		}

		public void SetChannelConfigurator(ConnectionBuilderConfigurator<IDictionary<TKey, TChannel>> configurator)
		{
			_configurator = configurator;
		}


		class DistinctConnectionBuilderDecorator :
			ConnectionBuilder<IDictionary<TKey, TChannel>>
		{
			readonly ConnectionBuilder<ICollection<TChannel>> _builder;
			readonly Fiber _fiber;
			readonly KeyAccessor<TChannel, TKey> _keyAccessor;

			public DistinctConnectionBuilderDecorator(ConnectionBuilder<ICollection<TChannel>> builder,
			                                          Fiber fiber, KeyAccessor<TChannel, TKey> keyAccessor)
			{
				_builder = builder;
				_fiber = fiber;
				_keyAccessor = keyAccessor;
			}

			public void AddChannel(Fiber fiber, Func<Fiber, Channel<IDictionary<TKey, TChannel>>> channelFactory)
			{
				Channel<IDictionary<TKey, TChannel>> channel = channelFactory(fiber);

				_builder.AddChannel(fiber, x => new DistinctChannel<TChannel, TKey>(_fiber, _keyAccessor, channel));
			}

			public void AddChannel<T>(Fiber fiber, Func<Fiber, Channel<T>> channelFactory)
			{
				throw new NotImplementedException("Cannot added non-descript channels to a distinct channel, sorry");
			}

			public void AddDisposable(IDisposable disposable)
			{
				_builder.AddDisposable(disposable);
			}
		}
	}
}