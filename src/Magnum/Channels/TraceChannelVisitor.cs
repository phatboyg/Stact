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
	using System.Diagnostics;
	using Extensions;

	public class TraceChannelVisitor :
		ChannelVisitor
	{
		protected override Channel<T> Visitor<T>(ConsumerChannel<T> channel)
		{
			Trace.WriteLine("ConsumerChannel<{0}>".FormatWith(typeof (T).Name));

			return base.Visitor(channel);
		}

		protected override Channel<T> Visitor<T>(FilterChannel<T> channel)
		{
			Trace.WriteLine("FilterChannel<{0}>".FormatWith(typeof (T).Name));

			return base.Visitor(channel);
		}

		protected override Channel<T> Visitor<T>(InstanceChannel<T> channel)
		{
			Trace.WriteLine("InstanceChannel<{0}>".FormatWith(typeof (T).Name));

			return base.Visitor(channel);
		}

		protected override Channel<T> Visitor<T>(IntervalChannel<T> channel)
		{
			Trace.WriteLine("IntervalChannel<{0}>, Interval = {1}".FormatWith(typeof (T).Name, channel.Interval));

			return base.Visitor(channel);
		}

		protected override Channel<T> Visitor<T>(InterceptorChannel<T> channel)
		{
			Trace.WriteLine("InterceptorChannel<{0}>".FormatWith(typeof (T).Name));

			return base.Visitor(channel);
		}

		protected override Channel<T> Visitor<T, TKey>(DistinctIntervalChannel<T, TKey> channel)
		{
			Trace.WriteLine("DistinctIntervalChannel<{0}>, Key = {1}, Interval = {2}".FormatWith(typeof (T).Name, typeof (TKey).Name,
				channel.Interval));

			return base.Visitor(channel);
		}

		protected override Channel<T> Visitor<T>(LastIntervalChannel<T> channel)
		{
			Trace.WriteLine("LastIntervalChannel<{0}>, Interval = {1}".FormatWith(typeof (T).Name, channel.Interval));

			return base.Visitor(channel);
		}

		protected override Channel<T> Visitor<T>(AsyncResultChannel<T> channel)
		{
			Trace.WriteLine("AsyncResultChannel<{0}>, {1}".FormatWith(typeof (T).Name, channel.IsCompleted ? "Complete" : "Pending"));

			return base.Visitor(channel);
		}

		protected override Channel<T> Visitor<T>(ShuntChannel<T> channel)
		{
			Trace.WriteLine("ShuntChannel<{0}>".FormatWith(typeof(T).Name));

			return base.Visitor<T>(channel);
		}

		protected override Channel<T> Visitor<T>(ChannelAdapter<T> channel)
		{
			Trace.WriteLine("ChannelAdapter<{0}>".FormatWith(typeof(T).Name));

			return base.Visitor<T>(channel);
		}

		protected override Channel<T> Visitor<T>(PublishSubscribeChannel<T> channel)
		{
			Trace.WriteLine("PublishSubscribeChannel<{0}>, {1} subscribers".FormatWith(typeof (T).Name, channel.Subscribers.Length));

			return base.Visitor(channel);
		}

		protected override Channel<TInput> Visitor<TInput, TOutput>(ConvertChannel<TInput, TOutput> channel)
		{
			Trace.WriteLine("ConvertChannel<{0}>, Output: {1}".FormatWith(typeof (TInput).Name, typeof (TOutput).Name));

			return base.Visitor(channel);
		}

		protected override Channel<T> Visitor<T>(Channel<T> channel)
		{
			Trace.WriteLine("Channel<{0}>".FormatWith(typeof (T).Name));

			return base.Visitor(channel);
		}

		protected override UntypedChannel Visitor(UntypedChannel channel)
		{
			Trace.WriteLine("UntypedChannel: {0}".FormatWith(channel.GetType().Name));

			return base.Visitor(channel);
		}

		protected override UntypedChannel Visitor(UntypedChannelRouter channel)
		{
			Trace.WriteLine("UntypedChannelRouter: {0} subscribers".FormatWith(channel.Subscribers.Length));

			return base.Visitor(channel);
		}

		protected override UntypedChannel Visitor<T>(TypedChannelAdapter<T> channel)
		{
			Trace.WriteLine("TypedChannelAdapter<{0}>".FormatWith(typeof(T).Name));

			return base.Visitor<T>(channel);
		}

		protected override ChannelProvider<T> Visitor<T>(ChannelProvider<T> provider)
		{
			Trace.WriteLine("ChannelProvider<{0}>".FormatWith(typeof (T).Name));

			return base.Visitor(provider);
		}

		protected override ChannelProvider<T> Visitor<T>(DelegateChannelProvider<T> provider)
		{
			Trace.WriteLine("DelegateChannelProvider<{0}>".FormatWith(typeof (T).Name));

			return base.Visitor(provider);
		}

		protected override ChannelProvider<T> Visitor<T, TKey>(KeyedChannelProvider<T, TKey> provider)
		{
			Trace.WriteLine("KeyedChannelProvider<{0}>, Key = {1}".FormatWith(typeof (T).Name, typeof (TKey).Name));

			return base.Visitor(provider);
		}

		protected override ChannelProvider<T> Visitor<T>(ThreadStaticChannelProvider<T> provider)
		{
			Trace.WriteLine("ThreadStaticChannelProvider<{0}>".FormatWith(typeof (T).Name));

			return base.Visitor(provider);
		}

		protected override InterceptorProvider<T> Visitor<T>(InterceptorProvider<T> provider)
		{
			Trace.WriteLine("InterceptorProvider<{0}>".FormatWith(typeof (T).Name));

			return base.Visitor(provider);
		}
	}
}