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
	using System.Linq;
	using Extensions;
	using Fibers;
	using Reflection;

	public class RemoveChannelVisitor :
		ChannelVisitor
	{
		private readonly HashSet<Channel> _channels;

		public RemoveChannelVisitor(IEnumerable<Channel> channels)
		{
			_channels = new HashSet<Channel>(channels);
		}

		public void RemoveFrom<T>(Channel<T> channel)
		{
			Visit(channel);

			if (_channels.Count > 0)
				throw new InvalidOperationException("There were {0} channels that were not removed.".FormatWith(_channels.Count));
		}

		public void RemoveFrom(UntypedChannel channel)
		{
			Visit(channel);

			if (_channels.Count > 0)
				throw new InvalidOperationException("There were {0} channels that were not removed.".FormatWith(_channels.Count));
		}

		public override Channel<T> Visit<T>(Channel<T> channel)
		{
			Channel<T> result = this.FastInvoke<RemoveChannelVisitor, Channel<T>>("Visitor", channel);

			return result;
		}

		protected override Channel<T> Visitor<T>(Channel<T> channel)
		{
			if (_channels.Contains(channel))
			{
				_channels.Remove(channel);
				return null;
			}

			return channel;
		}

		protected override Channel<T> Visitor<T>(ChannelAdapter<T> channel)
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

		protected override Channel<TInput> Visitor<TInput, TOutput>(ConvertChannel<TInput, TOutput> channel)
		{
			if (_channels.Contains(channel.Output))
			{
				_channels.Remove(channel.Output);
				return null;
			}

			Channel<TOutput> output = Visit(channel.Output);
			if (output == null)
				return null;

			if (output != channel.Output)
				return new ConvertChannel<TInput, TOutput>(new SynchronousFiber(), output, channel.Converter);

			return channel;
		}

		protected override UntypedChannel Visitor<T>(TypedChannelAdapter<T> channel)
		{
			Channel<T> original = channel.Output;

			Channel<T> replacement = Visit(original);

			if (_channels.Contains(replacement))
			{
				_channels.Remove(replacement);
				return null;
			}

			return original != replacement ? new TypedChannelAdapter<T>(replacement) : channel;
		}

		protected override UntypedChannel Visitor(UntypedChannelRouter channel)
		{
			var results = new List<UntypedChannel>();
			bool changed = false;

			foreach (UntypedChannel subscriber in channel.Subscribers)
			{
				UntypedChannel newSubscriber = Visit(subscriber);

				if (_channels.Contains(newSubscriber))
				{
					_channels.Remove(newSubscriber);
					newSubscriber = null;
				}

				if (newSubscriber == null || newSubscriber != subscriber)
				{
					changed = true;
					if (newSubscriber == null)
						continue;
				}

				results.Add(newSubscriber);
			}

			return changed ? new UntypedChannelRouter(results) : channel;
		}

		protected override UntypedChannel Visitor(UntypedChannelAdapter channel)
		{
			UntypedChannel original = channel.Output;

			UntypedChannel replacement = Visit(original);

			if (_channels.Contains(replacement))
			{
				_channels.Remove(replacement);
				replacement = null;
			}

			if (replacement == null)
			{
				replacement = new ShuntChannel();
			}

			if (replacement != original)
			{
				channel.ChangeOutputChannel(original, replacement);
			}

			return channel;
		}

		protected override Channel<T> Visitor<T>(PublishSubscribeChannel<T> channel)
		{
			bool changed;
			Channel<T>[] subscribers = VisitSubscribers(channel.Subscribers, out changed).ToArray();
			if (subscribers.Length == 1)
				return subscribers[0];

			if (changed)
				return new PublishSubscribeChannel<T>(subscribers);

			return channel;
		}

		private Channel<T>[] VisitSubscribers<T>(IEnumerable<Channel<T>> subscribers, out bool changed)
		{
			var results = new List<Channel<T>>();

			changed = false;
			foreach (Channel<T> subscriber in subscribers)
			{
				Channel<T> result = Visit(subscriber);
				if (result == null)
				{
					changed = true;
					continue;
				}

				if (_channels.Contains(result))
				{
					_channels.Remove(result);
					changed = true;
					continue;
				}

				results.Add(result);
			}

			return results.ToArray();
		}
	}
}