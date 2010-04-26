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

	public class RemoveChannelBinder
	{
		private readonly HashSet<Channel> _channels;

		public RemoveChannelBinder(IEnumerable<Channel> channels)
		{
			_channels = new HashSet<Channel>(channels);
		}

		public void Unbind<T>(Channel<T> channel)
		{
			Channel<T> result = Visit(channel);

			if (_channels.Count > 0)
				throw new InvalidOperationException("There were {0} channels that were not removed.".FormatWith(_channels.Count));
		}

		protected virtual Channel<T> Visit<T>(Channel<T> channel)
		{
			Channel<T> result = this.FastInvoke<RemoveChannelBinder, Channel<T>>("Visitor", channel);

			return result;
		}


		protected virtual Channel<T> Visitor<T>(Channel<T> channel)
		{
			return channel;
		}

		protected virtual Channel<T> Visitor<T>(ChannelAdapter<T> channel)
		{
			Channel<T> output = channel.Output;

			if (_channels.Contains(output))
			{
				var shunt = new ShuntChannel<T>();
				channel.ChangeOutputChannel(output, shunt);

				_channels.Remove(output);
			}

			Channel<T> newOutput = Visit(output);

			if (newOutput != output)
			{
				channel.ChangeOutputChannel(output, newOutput);
			}

			return channel;
		}

		protected virtual Channel<T> Visitor<T>(PublishSubscribeChannel<T> channel)
		{
			bool changed;
			var subscribers = VisitSubscribers(channel.Subscribers, out changed).ToArray();
			if(changed)
				return new PublishSubscribeChannel<T>(channel.Fiber, subscribers);

			return channel;
		}

		private Channel<T>[] VisitSubscribers<T>(IEnumerable<Channel<T>> subscribers, out bool changed)
		{
			List<Channel<T>> results = new List<Channel<T>>();

			changed = false;
			foreach (Channel<T> subscriber in subscribers)
			{
				Channel<T> result = Visit(subscriber);

				if(_channels.Contains(result))
				{
					changed = true;
					_channels.Remove(result);
					continue;
				}

				results.Add(result);
			}

			return results.ToArray();
		}
	}
}