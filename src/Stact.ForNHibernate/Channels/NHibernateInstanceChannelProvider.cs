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
namespace Stact.ForNHibernate.Channels
{
	using Fibers;
	using Stact.Channels;


	/// <summary>
	/// Gets an instance of a class from the InstanceProvider and returns the channel
	/// from that class
	/// </summary>
	/// <typeparam name="TInstance">The instance type</typeparam>
	/// <typeparam name="TChannel">The channel type</typeparam>
	/// <typeparam name="TKey"></typeparam>
	public class NHibernateInstanceChannelProvider<TInstance, TChannel, TKey> :
		ChannelProvider<TChannel>
		where TInstance : class
	{
		readonly ChannelAccessor<TInstance, TChannel> _channelAccessor;
		readonly FiberProvider<TKey> _fiberProvider;
		readonly InstanceChannelPolicy<TInstance, TChannel> _instanceChannelPolicy;
		readonly KeyAccessor<TChannel, TKey> _keyAccessor;
		readonly SessionProvider<TChannel> _sessionProvider;

		public NHibernateInstanceChannelProvider(FiberProvider<TKey> fiberProvider, SessionProvider<TChannel> sessionProvider,
		                                         KeyAccessor<TChannel, TKey> keyAccessor,
		                                         ChannelAccessor<TInstance, TChannel> channelAccessor,
		                                         InstanceChannelPolicy<TInstance, TChannel> instanceChannelPolicy)
		{
			Guard.AgainstNull(fiberProvider);
			Guard.AgainstNull(sessionProvider);
			Guard.AgainstNull(keyAccessor);
			Guard.AgainstNull(channelAccessor);
			Guard.AgainstNull(instanceChannelPolicy);

			_fiberProvider = fiberProvider;
			_sessionProvider = sessionProvider;
			_keyAccessor = keyAccessor;
			_channelAccessor = channelAccessor;
			_instanceChannelPolicy = instanceChannelPolicy;
		}

		public Channel<TChannel> GetChannel(TChannel message)
		{
			TKey key = _keyAccessor(message);

			Fiber fiber = _fiberProvider.GetFiber(key);

			return new NHibernateInstanceChannel<TInstance, TChannel, TKey>(fiber, _sessionProvider, _keyAccessor,
			                                                                _channelAccessor, _instanceChannelPolicy);
		}
	}
}