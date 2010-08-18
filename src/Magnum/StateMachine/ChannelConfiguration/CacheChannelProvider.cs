// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.StateMachine.ChannelConfiguration
{
	using Fibers;
	using Magnum.Channels;


	/// <summary>
	/// Gets an instance of a class from the InstanceProvider and returns the channel
	/// from that class
	/// </summary>
	/// <typeparam name="TChannel">The channel type</typeparam>
	/// <typeparam name="TKey"></typeparam>
	public class CacheChannelProvider<TChannel, TKey> :
		ChannelProvider<TChannel>
	{
		readonly ChannelProvider<TChannel> _channelProvider;
		readonly FiberProvider<TKey> _fiberProvider;
		readonly KeyAccessor<TChannel, TKey> _keyAccessor;

		public CacheChannelProvider(FiberProvider<TKey> fiberProvider, ChannelProvider<TChannel> channelProvider,
		                            KeyAccessor<TChannel, TKey> keyAccessor)
		{
			Guard.AgainstNull(fiberProvider);
			Guard.AgainstNull(channelProvider);
			Guard.AgainstNull(keyAccessor);

			_fiberProvider = fiberProvider;
			_channelProvider = channelProvider;
			_keyAccessor = keyAccessor;
		}

		public Channel<TChannel> GetChannel(TChannel message)
		{
			TKey key = _keyAccessor(message);

			Fiber fiber = _fiberProvider.GetFiber(key);

			var channel = new InstanceChannel<TChannel>(fiber, _channelProvider);

			return channel;
		}
	}
}