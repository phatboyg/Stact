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
	using System.Threading;
	using Stact.Internal;


	public class SynchronizedChannelConfiguratorImpl<TChannel> :
		SynchronizedChannelConfigurator<TChannel>,
		ConnectionBuilderConfigurator<TChannel>
	{
		readonly SynchronizationContext _synchronizationContext;
		ConnectionBuilderConfigurator<TChannel> _configurator;
		object _state;

		public SynchronizedChannelConfiguratorImpl()
		{
			_synchronizationContext = SynchronizationContext.Current;
		}

		public SynchronizedChannelConfiguratorImpl(SynchronizationContext synchronizationContext)
		{
			_synchronizationContext = synchronizationContext;
		}

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel configurator was setup");

			_configurator.ValidateConfiguration();
		}

		public void Configure(ConnectionBuilder<TChannel> builder)
		{
			if (_synchronizationContext == null)
				_configurator.Configure(builder);
			else
				ConfigureUsingDecoratedConnection(builder);
		}

		public void SetChannelConfigurator(ConnectionBuilderConfigurator<TChannel> configurator)
		{
			_configurator = configurator;
		}


		public SynchronizedChannelConfigurator<TChannel> WithState(object state)
		{
			_state = state;

			return this;
		}

		void ConfigureUsingDecoratedConnection(ConnectionBuilder<TChannel> builder)
		{
			var synchronizedConnection = new SynchronizedConnectionBuilderDecorator(builder, _synchronizationContext,
			                                                                        _state);

			_configurator.Configure(synchronizedConnection);
		}


		class SynchronizedConnectionBuilderDecorator :
			ConnectionBuilder<TChannel>
		{
			readonly ConnectionBuilder<TChannel> _builder;
			readonly object _state;
			readonly SynchronizationContext _synchronizationContext;

			public SynchronizedConnectionBuilderDecorator(ConnectionBuilder<TChannel> builder,
			                                              SynchronizationContext synchronizationContext, object state)
			{
				_builder = builder;
				_synchronizationContext = synchronizationContext;
				_state = state;
			}

			public void AddChannel(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory)
			{
				Channel<TChannel> channel = channelFactory(new SynchronousFiber());

				_builder.AddChannel<TChannel>(fiber,
				                              x => new SynchronizedChannel<TChannel>(x, channel, _synchronizationContext, _state));
			}

			public void AddChannel<T>(Fiber fiber, Func<Fiber, Channel<T>> channelFactory)
			{
				Channel<T> channel = channelFactory(new SynchronousFiber());

				_builder.AddChannel(fiber, x => new SynchronizedChannel<T>(x, channel, _synchronizationContext, _state));
			}

			public void AddDisposable(IDisposable disposable)
			{
				_builder.AddDisposable(disposable);
			}
		}
	}
}