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
namespace Magnum.Channels.Configuration.Internal
{
	using System;
	using System.Collections.Generic;
	using Fibers;


	public class DistinctChannelConfiguratorImpl<TChannel, TKey> :
		FiberModelConfigurator<DistinctChannelConfigurator<TChannel, TKey>>,
		DistinctChannelConfigurator<TChannel, TKey>,
		ChannelConfigurator<ICollection<TChannel>>
	{
		readonly KeyAccessor<TChannel, TKey> _keyAccessor;
		ChannelConfigurator<IDictionary<TKey,TChannel>> _configurator;

		public DistinctChannelConfiguratorImpl(KeyAccessor<TChannel, TKey> keyAccessor)
		{
			_keyAccessor = keyAccessor;

			ExecuteOnThreadPoolFiber();
		}

		public void Configure(ChannelConfiguratorConnection<ICollection<TChannel>> connection)
		{
			Fiber fiber = GetConfiguredFiber(connection);

			_configurator.Configure(new DistinctChannelConfiguratorConnection(connection, fiber, _keyAccessor));
		}

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel configurator was setup");

			_configurator.ValidateConfiguration();
		}

		public void SetChannelConfigurator(ChannelConfigurator<IDictionary<TKey, TChannel>> configurator)
		{
			_configurator = configurator;
		}


		class DistinctChannelConfiguratorConnection :
			ChannelConfiguratorConnection<IDictionary<TKey, TChannel>>
		{
			readonly ChannelConfiguratorConnection<ICollection<TChannel>> _connection;
			readonly Fiber _fiber;
			readonly KeyAccessor<TChannel, TKey> _keyAccessor;

			public DistinctChannelConfiguratorConnection(ChannelConfiguratorConnection<ICollection<TChannel>> connection,
			                                             Fiber fiber, KeyAccessor<TChannel, TKey> keyAccessor)
			{
				_connection = connection;
				_fiber = fiber;
				_keyAccessor = keyAccessor;
			}

			public void AddChannel(Fiber fiber, Func<Fiber, Channel<IDictionary<TKey, TChannel>>> channelFactory)
			{
				Channel<IDictionary<TKey, TChannel>> channel = channelFactory(fiber);

				_connection.AddChannel(fiber, x => new DistinctChannel<TChannel, TKey>(_fiber, _keyAccessor, channel));
			}

			public void AddChannel<T>(Fiber fiber, Func<Fiber, Channel<T>> channelFactory)
			{
				throw new NotImplementedException("Cannot added non-descript channels to a distinct channel, sorry");
			}

			public void AddDisposable(IDisposable disposable)
			{
				_connection.AddDisposable(disposable);
			}
		}
	}
}