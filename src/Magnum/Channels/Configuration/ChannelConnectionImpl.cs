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
namespace Magnum.Channels.Configuration
{
	using System;
	using System.Collections.Generic;


	public class ChannelConnectionImpl :
		ChannelConnection
	{
		readonly UntypedChannel _channel;
		readonly HashSet<Channel> _connectedChannels;

		bool _disposed;

		public ChannelConnectionImpl(UntypedChannel channel, HashSet<Channel> connectedChannels)
		{
			_channel = channel;
			_connectedChannels = connectedChannels;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Disconnect()
		{
			if (_connectedChannels.Count == 0)
				return;

			new DisconnectChannelVisitor(_connectedChannels).DisconnectFrom(_channel);

			_connectedChannels.Clear();
		}

		void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			if (disposing)
				Disconnect();

			_disposed = true;
		}

		~ChannelConnectionImpl()
		{
			Dispose(false);
		}
	}


	public class ChannelConnectionImpl<T> :
		ChannelConnection
	{
		readonly Channel<T> _channel;
		readonly HashSet<Channel> _connectedChannels;

		bool _disposed;

		public ChannelConnectionImpl(Channel<T> channel, HashSet<Channel> connectedChannels)
		{
			_channel = channel;
			_connectedChannels = connectedChannels;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Disconnect()
		{
			if (_connectedChannels.Count == 0)
				return;

			new DisconnectChannelVisitor(_connectedChannels).DisconnectFrom(_channel);

			_connectedChannels.Clear();
		}

		void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			if (disposing)
				Disconnect();

			_disposed = true;
		}

		~ChannelConnectionImpl()
		{
			Dispose(false);
		}
	}
}