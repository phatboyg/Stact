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
	using Extensions;
	using Logging;
	using Reflection;

	public class ChannelVisitor
	{
		private readonly ILogger _log = Logger.GetLogger<ChannelVisitor>();

		public virtual Channel<T> Visit<T>(Channel<T> channel)
		{
			Channel<T> result = this.FastInvoke<ChannelVisitor, Channel<T>>("Visitor", channel);

			return result;
		}

		public virtual ChannelProvider<T> Visit<T>(ChannelProvider<T> provider)
		{
			ChannelProvider<T> result = this.FastInvoke<ChannelVisitor, ChannelProvider<T>>("Visitor", provider);

			return result;
		}

		public virtual InterceptorProvider<T> Visit<T>(InterceptorProvider<T> provider)
		{
			InterceptorProvider<T> result = this.FastInvoke<ChannelVisitor, InterceptorProvider<T>>("Visitor", provider);

			return result;
		}

		protected virtual Channel<T> Visitor<T>(ConsumerChannel<T> channel)
		{
			return channel;
		}

		protected virtual Channel<T> Visitor<T>(ShuntChannel<T> channel)
		{
			return channel;
		}

		protected virtual Channel<T> Visitor<T>(FilterChannel<T> channel)
		{
			Visit(channel.Output);

			return channel;
		}


		protected virtual Channel<T> Visitor<T>(ChannelAdapter<T> channel)
		{
			Visit(channel.Output);

			return channel;
		}

		protected virtual Channel<T> Visitor<T>(InstanceChannel<T> channel)
		{
			Visit(channel.Provider);

			return channel;
		}

		protected virtual Channel<T> Visitor<T>(IntervalChannel<T> channel)
		{
			Visit(channel.Output);

			return channel;
		}

		protected virtual Channel<T> Visitor<T>(InterceptorChannel<T> channel)
		{
			Visit(channel.Output);

			Visit(channel.InterceptorProvider);

			return channel;
		}

		protected virtual Channel<T> Visitor<T, TKey>(DistinctIntervalChannel<T, TKey> channel)
		{
			Visit(channel.Output);

			return channel;
		}

		protected virtual Channel<T> Visitor<T>(LastIntervalChannel<T> channel)
		{
			Visit(channel.Output);

			return channel;
		}

		protected virtual Channel<T> Visitor<T>(AsyncResultChannel<T> channel)
		{
			Visit(channel.Output);

			return channel;
		}

		protected virtual Channel<T> Visitor<T>(PublishSubscribeChannel<T> channel)
		{
			channel.Subscribers.Each(subscriber => { Visit(subscriber); });

			return channel;
		}

		protected virtual Channel<TInput> Visitor<TInput, TOutput>(ConvertChannel<TInput, TOutput> channel)
		{
			Visit(channel.Output);

			return channel;
		}

		protected virtual Channel<T> Visitor<T>(Channel<T> channel)
		{
			_log.Warn(x => x.Write("Unknown channel implementation found: {0}", channel.GetType().FullName));

			return channel;
		}

		protected virtual ChannelProvider<T> Visitor<T>(ChannelProvider<T> provider)
		{
			return provider;
		}

		protected virtual ChannelProvider<T> Visitor<T>(DelegateChannelProvider<T> provider)
		{
			return provider;
		}

		protected virtual ChannelProvider<T> Visitor<T, TKey>(KeyedChannelProvider<T, TKey> provider)
		{
			Visit(provider.InstanceProvider);

			return provider;
		}

		protected virtual ChannelProvider<T> Visitor<T>(ThreadStaticChannelProvider<T> provider)
		{
			Visit(provider.InstanceProvider);

			return provider;
		}

		protected virtual InterceptorProvider<T> Visitor<T>(InterceptorProvider<T> provider)
		{
			return provider;
		}
	}
}