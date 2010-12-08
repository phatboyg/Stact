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


	public class ChannelConfiguratorImpl<TChannel> :
		ChannelConfigurator<TChannel>,
		ConnectionBuilderConfigurator
	{
		ConnectionBuilderConfigurator<TChannel> _configurator;

		public void SetChannelConfigurator(ConnectionBuilderConfigurator<TChannel> configurator)
		{
			_configurator = configurator;
		}

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel configurator was setup");

			_configurator.ValidateConfiguration();
		}

		public void Configure(ConnectionBuilder builder)
		{
			_configurator.Configure(new ChannelConnectionBuilderDecorator(builder));
		}


		class ChannelConnectionBuilderDecorator :
			ConnectionBuilder<TChannel>
		{
			readonly ConnectionBuilder _builder;

			public ChannelConnectionBuilderDecorator(ConnectionBuilder builder)
			{
				_builder = builder;
			}

			public void AddChannel(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory)
			{
				_builder.AddChannel(fiber, channelFactory);
			}

			public void AddChannel<T>(Fiber fiber, Func<Fiber, Channel<T>> channelFactory)
			{
				_builder.AddChannel(fiber, channelFactory);
			}

			public void AddDisposable(IDisposable disposable)
			{
				_builder.AddDisposable(disposable);
			}
		}
	}
}