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
	using Extensions;
	using Reflection;

	public class AddChannelBinder<TChannel>
	{
		private readonly Channel<TChannel> _newChannel;
		private bool _added;

		public AddChannelBinder(Channel<TChannel> newChannel)
		{
			_newChannel = newChannel;
		}

		public void BindTo<T>(Channel<T> channel)
		{
			Channel<T> result = Visit(channel);

			if (!_added)
				throw new InvalidOperationException("The binding operation failed: {0} to {1}".FormatWith(typeof (T).Name, typeof (TChannel).Name));
		}

		protected virtual Channel<T> Visit<T>(Channel<T> channel)
		{
			Channel<T> result = this.FastInvoke<AddChannelBinder<TChannel>, Channel<T>>("Visitor", channel);

			return result;
		}

		protected virtual Channel<T> Visitor<T>(Channel<T> channel)
		{
			return channel;
		}

		protected virtual Channel<T> Visitor<T>(ChannelAdapter<T> channel)
		{
			//Guard.IsTrue(x => x.IsAssignableFrom(typeof(TChannel)), typeof(T), "Type {0} is not assignable to {1}".FormatWith(typeof(T).Name, typeof(TChannel).Name));

			Channel<T> originalOutput = channel.Output;

			Channel<T> output = Visit(originalOutput);

			if (originalOutput != output)
			{
				channel.ChangeOutputChannel(originalOutput, output);
			}

			return channel;
		}

		protected virtual Channel<T> Visitor<T>(ShuntChannel<T> channel)
		{
			if (typeof (T) == typeof (TChannel))
			{
				_added = true;
				return (Channel<T>) _newChannel;
			}

			return channel;
		}

		protected virtual Channel<T> Visitor<T>(PublishSubscribeChannel<T> channel)
		{
			if (typeof (T) == typeof (TChannel))
			{
				return new PublishSubscribeChannel<T>(channel.Fiber, VisitSubscribers(channel.Subscribers));
			}

			return channel;
		}

		private IEnumerable<Channel<T>> VisitSubscribers<T>(IEnumerable<Channel<T>> recipients)
		{
			foreach (Channel<T> recipient in recipients)
			{
				Channel<T> newRecipient = Visit(recipient);

				if (newRecipient == (Channel<T>) _newChannel)
					throw new InvalidOperationException("The channel has already been added to the network");

				if (newRecipient != null)
					yield return newRecipient;
			}

			if (!_added)
			{
				_added = true;
				yield return (Channel<T>) _newChannel;
			}
		}
	}
}