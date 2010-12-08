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


	public class LastChannelConfiguratorImpl<TChannel> :
		FiberFactoryConfiguratorImpl<LastChannelConfigurator<TChannel>>,
		LastChannelConfigurator<TChannel>,
		ConnectionBuilderConfigurator<ICollection<TChannel>>
	{
		ConnectionBuilderConfigurator<TChannel> _configurator;

		public LastChannelConfiguratorImpl()
		{
			HandleOnCallingThread();
		}

		public void Configure(ConnectionBuilder<ICollection<TChannel>> builder)
		{
			Fiber fiber = this.GetFiberUsingConfiguredFactory(builder);

			_configurator.Configure(new LastConnectionBuilderDecorator(builder, fiber));
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


		class LastConnectionBuilderDecorator :
			ConnectionBuilder<TChannel>
		{
			readonly ConnectionBuilder<ICollection<TChannel>> _builder;
			readonly Fiber _fiber;

			public LastConnectionBuilderDecorator(ConnectionBuilder<ICollection<TChannel>> builder, Fiber fiber)
			{
				_builder = builder;
				_fiber = fiber;
			}

			public void AddChannel(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory)
			{
				Channel<TChannel> channel = channelFactory(fiber);

				_builder.AddChannel(fiber, x => new LastChannel<TChannel>(_fiber, channel));
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