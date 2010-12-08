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


	public class FilterChannelConfiguratorImpl<TChannel> :
		FiberFactoryConfiguratorImpl<FilterChannelConfigurator<TChannel>>,
		FilterChannelConfigurator<TChannel>,
		ConnectionBuilderConfigurator<TChannel>
	{
		readonly Filter<TChannel> _filter;
		ConnectionBuilderConfigurator<TChannel> _configurator;

		public FilterChannelConfiguratorImpl(Filter<TChannel> filter)
		{
			_filter = filter;

			HandleOnPoolFiber();
		}

		public void Configure(ConnectionBuilder<TChannel> builder)
		{
			Fiber fiber = this.GetFiberUsingConfiguredFactory(builder);

			var filterConnection = new FilterConnectionBuilderDecorator(builder, fiber, _filter);

			_configurator.Configure(filterConnection);
		}

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel configurator was setup");

			_configurator.ValidateConfiguration();
		}

		public void SetChannelConfigurator(ConnectionBuilderConfigurator<TChannel> configurator)
		{
			_configurator = configurator;
		}


		class FilterConnectionBuilderDecorator :
			ConnectionBuilder<TChannel>
		{
			readonly ConnectionBuilder<TChannel> _builder;
			readonly Fiber _fiber;
			readonly Filter<TChannel> _filter;

			public FilterConnectionBuilderDecorator(ConnectionBuilder<TChannel> builder, Fiber fiber,
			                                              Filter<TChannel> filter)
			{
				_builder = builder;
				_fiber = fiber;
				_filter = filter;
			}

			public void AddChannel(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory)
			{
				Channel<TChannel> channel = channelFactory(fiber);

				_builder.AddChannel(fiber, x => new FilterChannel<TChannel>(_fiber, channel, _filter));
			}

			public void AddChannel<T>(Fiber fiber, Func<Fiber, Channel<T>> channelFactory)
			{
				Channel<T> channel = channelFactory(fiber);

				Filter<T> filter = m => _filter((TChannel)(object)m);

				_builder.AddChannel(_fiber, x => new FilterChannel<T>(x, channel, filter));
			}

			public void AddDisposable(IDisposable disposable)
			{
				_builder.AddDisposable(disposable);
			}
		}
	}
}