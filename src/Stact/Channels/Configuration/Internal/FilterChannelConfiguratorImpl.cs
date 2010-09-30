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
namespace Stact.Channels.Configuration.Internal
{
	using System;
	using Fibers;
	using Fibers.Configuration;


	public class FilterChannelConfiguratorImpl<TChannel> :
		FiberConfiguratorImpl<FilterChannelConfigurator<TChannel>>,
		FilterChannelConfigurator<TChannel>,
		ChannelConfigurator<TChannel>
	{
		readonly Filter<TChannel> _filter;
		ChannelConfigurator<TChannel> _configurator;

		public FilterChannelConfiguratorImpl(Filter<TChannel> filter)
		{
			_filter = filter;

			HandleOnFiber();
		}

		public void Configure(ChannelConfiguratorConnection<TChannel> connection)
		{
			Fiber fiber = GetConfiguredFiber(connection);

			var filterConnection = new ChannelConfiguratorConnectionDecorator(connection, fiber, _filter);

			_configurator.Configure(filterConnection);
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


		class ChannelConfiguratorConnectionDecorator :
			ChannelConfiguratorConnection<TChannel>
		{
			readonly ChannelConfiguratorConnection<TChannel> _connection;
			readonly Fiber _fiber;
			readonly Filter<TChannel> _filter;

			public ChannelConfiguratorConnectionDecorator(ChannelConfiguratorConnection<TChannel> connection, Fiber fiber,
			                                              Filter<TChannel> filter)
			{
				_connection = connection;
				_fiber = fiber;
				_filter = filter;
			}

			public void AddChannel(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory)
			{
				Channel<TChannel> channel = channelFactory(fiber);

				_connection.AddChannel(fiber, x => new FilterChannel<TChannel>(_fiber, channel, _filter));
			}

			public void AddChannel<T>(Fiber fiber, Func<Fiber, Channel<T>> channelFactory)
			{
				Channel<T> channel = channelFactory(fiber);

				Filter<T> filter = m => _filter((TChannel)(object)m);

				_connection.AddChannel(_fiber, x => new FilterChannel<T>(x, channel, filter));
			}

			public void AddDisposable(IDisposable disposable)
			{
				_connection.AddDisposable(disposable);
			}
		}
	}
}