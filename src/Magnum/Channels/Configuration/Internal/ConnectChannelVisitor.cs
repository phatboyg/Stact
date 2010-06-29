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
	using System.Collections.Generic;
	using Extensions;
	using Fibers;
	using Reflection;
	using Visitors;


	public class ConnectChannelVisitor :
		ChannelVisitor
	{
		readonly UntypedChannel _newChannel;
		bool _added;

		public ConnectChannelVisitor(UntypedChannel newChannel)
		{
			_newChannel = newChannel;
		}

		public void ConnectTo(UntypedChannel channel)
		{
			UntypedChannel result = Visit(channel);

			if (!_added)
				throw new InvalidOperationException("The binding operation failed");
		}

		protected override UntypedChannel Visitor(BroadcastChannel channel)
		{
			var results = new List<UntypedChannel>();
			bool changed = false;

			foreach (UntypedChannel subscriber in channel.Listeners)
			{
				UntypedChannel newSubscriber = Visit(subscriber);

				if (newSubscriber == null || newSubscriber != subscriber)
				{
					changed = true;
					if (newSubscriber == null)
						continue;
				}

				results.Add(newSubscriber);
			}

			if (!_added)
			{
				_added = true;
				results.Add(_newChannel);
				changed = true;
			}

			if (changed)
				return new BroadcastChannel(results);

			return channel;
		}

		protected override UntypedChannel Visitor(ChannelAdapter channel)
		{
			UntypedChannel original = channel.Output;

			UntypedChannel replacement = Visit(original);

			if (!_added)
			{
				if (replacement.GetType() == typeof(ShuntChannel))
				{
					replacement = new BroadcastChannel(new[] {_newChannel});
					_added = true;
				}
			}

			if (original != replacement)
				channel.ChangeOutputChannel(original, replacement);

			return channel;
		}
	}


	public class ConnectChannelVisitor<TChannel> :
		ChannelVisitor
	{
		readonly Channel<TChannel> _newChannel;
		bool _added;

		public ConnectChannelVisitor(Channel<TChannel> newChannel)
		{
			_newChannel = newChannel;
		}

		public void ConnectTo<T>(Channel<T> channel)
		{
			Channel<T> result = Visit(channel);

			if (!_added)
			{
				throw new InvalidOperationException("The binding operation failed: {0} to {1}".FormatWith(typeof(T).Name,
				                                                                                          typeof(TChannel).Name));
			}
		}

		public void ConnectTo(UntypedChannel channel)
		{
			UntypedChannel result = Visit(channel);

			if (!_added)
				throw new InvalidOperationException("The binding operation failed: {0}".FormatWith(typeof(TChannel).Name));
		}

		public override Channel<T> Visit<T>(Channel<T> channel)
		{
			return this.FastInvoke<ConnectChannelVisitor<TChannel>, Channel<T>>("Visitor", channel);
		}

		protected override Channel<T> Visitor<T>(ChannelAdapter<T> channel)
		{
			Channel<T> original = channel.Output;

			Channel<T> replacement = Visit(original);


			if (!_added && typeof(T) == typeof(TChannel))
			{
				replacement = new BroadcastChannel<T>(new[] {replacement, GetChannel<T>()});
				_added = true;
			}

			if (original != replacement)
				channel.ChangeOutputChannel(original, replacement);

			return channel;
		}

		protected override UntypedChannel Visitor<T>(TypedChannelAdapter<T> channel)
		{
			Channel<T> original = channel.Output;

			Channel<T> replacement = Visit(original);

			if (!_added && typeof(T) == typeof(TChannel))
			{
				replacement = new BroadcastChannel<T>(new[] {replacement, GetChannel<T>()});
				_added = true;
			}

			if (original != replacement)
				return new TypedChannelAdapter<T>(replacement);

			return channel;
		}

		protected override UntypedChannel Visitor(BroadcastChannel channel)
		{
			var results = new List<UntypedChannel>();
			bool changed = false;

			foreach (UntypedChannel subscriber in channel.Listeners)
			{
				UntypedChannel newSubscriber = Visit(subscriber);

				if (newSubscriber == null || newSubscriber != subscriber)
				{
					changed = true;
					if (newSubscriber == null)
						continue;
				}

				results.Add(newSubscriber);
			}

			if (!_added)
			{
				_added = true;
				results.Add(GetUntypedChannel());
				changed = true;
			}

			if (changed)
				return new BroadcastChannel(results);

			return channel;
		}

		protected override UntypedChannel Visitor(ChannelAdapter channel)
		{
			UntypedChannel original = channel.Output;

			UntypedChannel replacement = Visit(original);

			if (!_added)
			{
				if (replacement.GetType() == typeof(ShuntChannel))
				{
					replacement = new BroadcastChannel(new[] {GetUntypedChannel()});
					_added = true;
				}
			}

			if (original != replacement)
				channel.ChangeOutputChannel(original, replacement);

			return channel;
		}

		protected override Channel<T> Visitor<T>(ShuntChannel<T> channel)
		{
			if (typeof(T) == typeof(TChannel))
			{
				_added = true;
				return GetChannel<T>();
			}

			return channel;
		}

		protected override Channel<T> Visitor<T>(BroadcastChannel<T> channel)
		{
			if (IsCompatibleType(typeof(T)))
				return new BroadcastChannel<T>(VisitSubscribers(channel.Listeners));

			return channel;
		}

		Channel<T> GetChannel<T>()
		{
			if (typeof(T) == typeof(TChannel))
				return (Channel<T>)_newChannel;

			return new ConvertChannel<T, TChannel>(new SynchronousFiber(), _newChannel);
		}

		UntypedChannel GetUntypedChannel()
		{
			return new TypedChannelAdapter<TChannel>(_newChannel);
		}

		IEnumerable<Channel<T>> VisitSubscribers<T>(IEnumerable<Channel<T>> recipients)
		{
			foreach (var recipient in recipients)
			{
				Channel<T> newRecipient = Visit(recipient);

				if (newRecipient == _newChannel)
					throw new InvalidOperationException("The channel has already been added to the network");

				if (newRecipient != null)
					yield return newRecipient;
			}

			if (!_added)
			{
				_added = true;
				yield return GetChannel<T>();
			}
		}

		static bool IsCompatibleType(Type type)
		{
			return typeof(TChannel).IsAssignableFrom(type);
		}
	}
}