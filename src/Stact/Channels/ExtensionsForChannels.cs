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
namespace Stact.Channels
{
	using System;
	using System.Collections.Generic;
	using Configuration;
	using Configuration.Internal;
	using Visitors;

	public static class ExtensionsForChannels
	{
		public static ChannelConnection Connect<T>(this Channel<T> channel, Action<ConnectionConfigurator<T>> subscriberActions)
		{
			Magnum.Guard.AgainstNull(channel, "channel");

			var subscriber = new ConnectionConfiguratorImpl<T>(channel);

			subscriberActions(subscriber);

			return subscriber.Complete();
		}

		public static ChannelConnection Connect(this UntypedChannel channel, Action<ConnectionConfigurator> subscriberActions)
		{
			Magnum.Guard.AgainstNull(channel, "channel");

			var subscriber = new ConnectionConfiguratorImpl(channel);

			subscriberActions(subscriber);

			return subscriber.Complete();
		}

		public static IEnumerable<Channel> Flatten<T>(this Channel<T> channel)
		{
			return new FlattenChannelVisitor().Flatten(channel);
		}

		public static IEnumerable<Channel> Flatten(this UntypedChannel channel)
		{
			return new FlattenChannelVisitor().Flatten(channel);
		}
	}
}