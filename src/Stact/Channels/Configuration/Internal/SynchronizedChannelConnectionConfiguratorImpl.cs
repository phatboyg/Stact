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
namespace Stact.Channels.Configuration.Internal
{
	using System;
	using System.Threading;
	using Fibers;


	public class SynchronizedChannelConnectionConfiguratorImpl<TChannel> :
		SynchronizedChannelConnectionConfigurator<TChannel>,
		ChannelConfigurator<TChannel>
	{
		ChannelConfigurator<TChannel> _configurator;
		object _state;
		readonly SynchronizationContext _synchronizationContext;

		public SynchronizedChannelConnectionConfiguratorImpl()
		{
			_synchronizationContext = SynchronizationContext.Current;
		}

		public SynchronizedChannelConnectionConfiguratorImpl(SynchronizationContext synchronizationContext)
		{
			_synchronizationContext = synchronizationContext;
		}

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel configurator was setup");

			_configurator.ValidateConfiguration();
		}

		public void Configure(ChannelConfiguratorConnection<TChannel> connection)
		{
			if (_synchronizationContext == null)
				_configurator.Configure(connection);
			else
				ConfigureUsingDecoratedConnection(connection);
		}

		public void SetChannelConfigurator(ChannelConfigurator<TChannel> configurator)
		{
			_configurator = configurator;
		}


		public SynchronizedChannelConnectionConfigurator<TChannel> WithState(object state)
		{
			_state = state;

			return this;
		}

		void ConfigureUsingDecoratedConnection(ChannelConfiguratorConnection<TChannel> connection)
		{
			var synchronizedConnection = new SynchronizedChannelConfiguratorConnection(connection, _synchronizationContext,
			                                                                           _state);

			_configurator.Configure(synchronizedConnection);
		}


		class SynchronizedChannelConfiguratorConnection :
			ChannelConfiguratorConnection<TChannel>
		{
			readonly ChannelConfiguratorConnection<TChannel> _connection;
			readonly object _state;
			readonly SynchronizationContext _synchronizationContext;

			public SynchronizedChannelConfiguratorConnection(ChannelConfiguratorConnection<TChannel> connection,
			                                                 SynchronizationContext synchronizationContext, object state)
			{
				_connection = connection;
				_synchronizationContext = synchronizationContext;
				_state = state;
			}

			public void AddChannel(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory)
			{
				Channel<TChannel> channel = channelFactory(new SynchronousFiber());

				_connection.AddChannel<TChannel>(fiber, x => new SynchronizedChannel<TChannel>(x, channel, _synchronizationContext, _state));
			}

			public void AddChannel<T>(Fiber fiber, Func<Fiber, Channel<T>> channelFactory)
			{
				Channel<T> channel = channelFactory(new SynchronousFiber());

				_connection.AddChannel(fiber, x => new SynchronizedChannel<T>(x, channel, _synchronizationContext, _state));
			}

			public void AddDisposable(IDisposable disposable)
			{
				_connection.AddDisposable(disposable);
			}
		}
	}
}