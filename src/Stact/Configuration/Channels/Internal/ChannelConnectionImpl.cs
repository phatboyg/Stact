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
	using System.Collections.Generic;


	/// <summary>
	/// The channel connection implementation
	/// </summary>
	public class ChannelConnectionImpl :
		ChannelConnectionBase,
		ChannelConnection,
		CreateChannelConnection
	{
		public ChannelConnectionImpl(UntypedChannel channel)
			: base(x => DisconnectChannels(channel, x))
		{
		}

		static void DisconnectChannels(UntypedChannel channel, IEnumerable<Channel> channels)
		{
			new DisconnectChannelVisitor(channels).DisconnectFrom(channel);
		}
	}


	public class ChannelConnectionImpl<T> :
		ChannelConnectionBase,
		ChannelConnection,
		CreateChannelConnection
	{
		public ChannelConnectionImpl(Channel<T> channel)
			: base(x => DisconnectChannels(channel, x))
		{
		}

		static void DisconnectChannels(Channel<T> channel, IEnumerable<Channel> channels)
		{
			new DisconnectChannelVisitor(channels).DisconnectFrom(channel);
		}
	}
}