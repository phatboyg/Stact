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
namespace Magnum.Channels.Internal
{
	using System;
	using System.Collections.Generic;

	public class UntypedChannelSubscription :
		ChannelSubscription
	{
		private readonly UntypedChannel _channel;
		private readonly HashSet<Channel> _subscribers;

		private bool _disposed;

		public UntypedChannelSubscription(UntypedChannel channel, HashSet<Channel> subscribers)
		{
			_channel = channel;
			_subscribers = subscribers;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void RemoveSubscribers()
		{
			new RemoveChannelSubscribers(_subscribers).RemoveFrom(_channel);

			_subscribers.Clear();
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				RemoveSubscribers();
			}

			_disposed = true;
		}

		~UntypedChannelSubscription()
		{
			Dispose(false);
		}
	}
}