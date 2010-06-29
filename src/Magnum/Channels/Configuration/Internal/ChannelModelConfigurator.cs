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
	using System;
	using System.Threading;
	using Fibers;


	public class ChannelModelConfigurator<T, TChannel> :
		FiberModelConfigurator<T>
		where T : class
	{
		SynchronizationContext _synchronizationContext;


		public T UseCurrentSychronizationContext()
		{
			_synchronizationContext = SynchronizationContext.Current;

			return this as T;
		}

		public T WithSynchronizationContext(SynchronizationContext synchronizationContext)
		{
			_synchronizationContext = synchronizationContext;

			return this as T;
		}

		public Channel<TChannel> CreateChannel(Func<Channel<TChannel>> channelBuilder)
		{
			if (_synchronizationContext == null)
				return channelBuilder();

			FiberFactory originalFiberFactory = _fiberFactory;

			UseProducerThread();

			Channel<TChannel> channel = channelBuilder();

			return new SynchronizedChannel<TChannel>(originalFiberFactory(), channel, _synchronizationContext);
		}
	}
}