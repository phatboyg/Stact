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
	using Magnum;
	using Magnum.Extensions;
	using Stact.Internal;


	public class TypedConnectionBuilderConfigurator<TChannel> :
		ConnectionBuilderConfigurator,
		ConnectionBuilderConfigurator<TChannel>
	{
		readonly Channel<TChannel> _channel;

		public TypedConnectionBuilderConfigurator(Channel<TChannel> channel)
		{
			Guard.AgainstNull(channel);

			_channel = channel;
		}

		public void ValidateConfiguration()
		{
		}

		public void Configure(ConnectionBuilder builder)
		{
			builder.AddChannel(null, fiber => _channel);
		}

		public void Configure(ConnectionBuilder<TChannel> builder)
		{
			builder.AddChannel(null, fiber => _channel);
		}
	}


	public class TypedConnectionBuilderConfigurator<T, TChannel> :
		ConnectionBuilderConfigurator<T>
	{
		readonly Channel<TChannel> _channel;

		public TypedConnectionBuilderConfigurator(Channel<TChannel> channel)
		{
			_channel = channel;
		}

		public void Configure(ConnectionBuilder<T> builder)
		{
			var fiber = new SynchronousFiber();

			builder.AddChannel<T>(fiber, x => new ConvertChannel<T, TChannel>(_channel));
		}

		public void ValidateConfiguration()
		{
			if (_channel == null)
				throw new ChannelConfigurationException("A null channel was specified");

			if (!typeof(TChannel).Implements<T>())
			{
				throw new ChannelConfigurationException(typeof(TChannel), "The type {0} is not implemented by the channel type"
				                                                          	.FormatWith(typeof(T).ToShortTypeName()));
			}
		}
	}
}