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
	using System.Collections.Generic;
	using System.Linq;
	using Extensions;
	using Reflection;

	public class ChannelRouter :
		UntypedChannel
	{
		private readonly Dictionary<Type, Channel> _channels;

		public ChannelRouter(IEnumerable<Channel> channels)
		{
			Guard.AgainstNull(channels, "channels");

			_channels = new Dictionary<Type, Channel>();

			channels.Each(channel =>
				{
					Guard.AgainstNull(channel, "channel");

					Type type = channel.GetType();
					Guard.IsTrue(x => x.Implements(typeof (Channel<>)), type, "Objects must implement Channel<T>: " + type.Name);

					Type channelType = type.GetGenericTypeDeclarations(typeof (Channel<>)).Single();
					_channels.Add(channelType, channel);
				});
		}

		public void Send<T>(T message)
		{
			_channels.Each(channel =>
				{
					if (channel.GetType().IsAssignableFrom(typeof (T)))
					{
						channel.Value.FastInvoke(new[] {channel.Key}, "Send", message);
					}
				});
		}
	}
}