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


	public class LastChannelConfiguratorImpl<TChannel> :
		FiberModelConfigurator<LastChannelConfigurator<TChannel>>,
		LastChannelConfigurator<TChannel>,
		ChannelConfigurator<ICollection<TChannel>>
	{
		ChannelConfigurator<TChannel> _configurator;

		public LastChannelConfiguratorImpl()
		{
			ExecuteOnProducerThread();
		}

		public void Configure(ChannelConfiguratorConnection<ICollection<TChannel>> connection)
		{
			Fiber fiber = GetConfiguredFiber(connection);

			_configurator.Configure(new LastChannelConfiguratorConnection(connection, fiber));
		}

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel configurator was setup");

			_configurator.ValidateConfiguration();
		}

		public void SetChannelConfigurator(ChannelConfigurator<TChannel> configurator)
		{
			_configurator = configurator;
		}


		class LastChannelConfiguratorConnection :
			ChannelConfiguratorConnection<TChannel>
		{
			readonly ChannelConfiguratorConnection<ICollection<TChannel>> _connection;
			readonly Fiber _fiber;

			public LastChannelConfiguratorConnection(ChannelConfiguratorConnection<ICollection<TChannel>> connection,
			                                         Fiber fiber)
			{
				_connection = connection;
				_fiber = fiber;
			}

			public void AddChannel(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory)
			{
				Channel<TChannel> channel = channelFactory(fiber);

				_connection.AddChannel(fiber, x => new LastChannel<TChannel>(_fiber, channel));
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