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
namespace Magnum.Infrastructure.Channels
{
	using Fibers;
	using Magnum.Channels;


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
		readonly FiberFactory _fiberFactory;
		readonly KeyAccessor<TChannel, TKey> _keyAccessor;
		readonly InstanceProvider<TInstance, TChannel> _missingInstanceProvider;
		readonly SessionProvider<TChannel> _sessionProvider;

		public NHibernateInstanceChannelProvider(FiberFactory fiberFactory, SessionProvider<TChannel> sessionProvider,
		                                         KeyAccessor<TChannel, TKey> keyAccessor,
		                                         ChannelAccessor<TInstance, TChannel> channelAccessor,
		                                         InstanceProvider<TInstance, TChannel> missingInstanceProvider)
		{
			Guard.AgainstNull(missingInstanceProvider);
			Guard.AgainstNull(channelAccessor);

			_fiberFactory = fiberFactory;
			_sessionProvider = sessionProvider;
			_keyAccessor = keyAccessor;
			_channelAccessor = channelAccessor;
			_missingInstanceProvider = missingInstanceProvider;
		}

		public Channel<TChannel> GetChannel(TChannel message)
		{
			var channel = new NHibernateInstanceChannel<TInstance, TChannel, TKey>(_fiberFactory(), _sessionProvider,
			                                                                       _keyAccessor, _channelAccessor,
			                                                                       _missingInstanceProvider);


			return channel;
		}
	}
}