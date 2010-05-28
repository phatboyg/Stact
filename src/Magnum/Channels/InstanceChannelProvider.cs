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
namespace Magnum.Channels
{
	using System;

	/// <summary>
	/// Creates an instance of a class and returns the channel from the class
	/// </summary>
	/// <typeparam name="TConsumer">The consumer type</typeparam>
	/// <typeparam name="TChannel">The channel type</typeparam>
	public class InstanceChannelProvider<TConsumer, TChannel> :
		ChannelProvider<TChannel>
	{
		private readonly ChannelAccessor<TConsumer, TChannel> _accessor;
		private readonly Func<TChannel, Channel<TChannel>> _channelProvider;
		private readonly Func<TChannel, TConsumer> _factory;

		public InstanceChannelProvider(Func<TChannel, TConsumer> factory, ChannelAccessor<TConsumer, TChannel> accessor)
		{
			Guard.AgainstNull(factory);
			Guard.AgainstNull(accessor);

			_accessor = accessor;
			_factory = factory;
		}

		public Channel<TChannel> GetChannel(TChannel message)
		{
			Channel<TChannel> channel = _accessor(_factory(message));
			if (channel == null)
				throw new InvalidOperationException("The channel on the consumer " + typeof (TConsumer).Name + " is null: "
				                                    + typeof (TChannel).Name);

			return channel;
		}
	}
}