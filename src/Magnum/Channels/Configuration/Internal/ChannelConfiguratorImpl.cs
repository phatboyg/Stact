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
	using Extensions;
	using Fibers;


	public class ChannelConfiguratorImpl :
		ChannelConfigurator
	{
		readonly UntypedChannel _channel;

		public ChannelConfiguratorImpl(UntypedChannel channel)
		{
			Guard.AgainstNull(channel);

			_channel = channel;
		}

		public void ValidateConfiguration()
		{
		}

		public void Configure(ChannelConfiguratorConnection connection)
		{
			connection.AddChannel(null, fiber => _channel);
		}
	}


	public class ChannelConfiguratorImpl<TChannel> :
		ChannelConfigurator,
		ChannelConfigurator<TChannel>
	{
		readonly Channel<TChannel> _channel;

		public ChannelConfiguratorImpl(Channel<TChannel> channel)
		{
			Guard.AgainstNull(channel);

			_channel = channel;
		}

		public void ValidateConfiguration()
		{
		}

		public void Configure(ChannelConfiguratorConnection connection)
		{
			connection.AddChannel(null, fiber => _channel);
		}

		public void Configure(ChannelConfiguratorConnection<TChannel> connection)
		{
			connection.AddChannel(null, fiber => _channel);
		}
	}


	public class ChannelConfiguratorImpl<T, TChannel> :
		ChannelConfigurator<T>
	{
		readonly Channel<TChannel> _channel;

		public ChannelConfiguratorImpl(Channel<TChannel> channel)
		{
			_channel = channel;
		}

		public void Configure(ChannelConfiguratorConnection<T> connection)
		{
			var fiber = new SynchronousFiber();

			connection.AddChannel<T>(fiber, x => new ConvertChannel<T, TChannel>(x, _channel));
		}

		public void ValidateConfiguration()
		{
			if (_channel == null)
				throw new ChannelConfigurationException("A null channel was specified");

			if(!typeof(TChannel).Implements<T>())
				throw new ChannelConfigurationException(typeof(TChannel), "The type {0} is not implemented by the channel type"
					.FormatWith(typeof(T).ToShortTypeName()));
		}
	}
}