// Copyright 2010-2013 Chris Patterson
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
namespace Stact.Visitors
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Internal;
    using Internals.Extensions;


    public class TraceChannelVisitor :
        ChannelVisitor
    {
        protected override Channel<T> Visit<T>(ConsumerChannel<T> channel)
        {
            Trace.WriteLine(string.Format("ConsumerChannel<{0}>", typeof(T).GetTypeName()));

            return base.Visit(channel);
        }

        protected override Channel<T> Visit<T>(SelectiveConsumerChannel<T> channel)
        {
            Trace.WriteLine(string.Format("SelectiveConsumerChannel<{0}>", typeof(T).GetTypeName()));

            return base.Visit(channel);
        }

        protected override ChannelProvider<TChannel> Visit<TConsumer, TChannel>(
            InstanceChannelProvider<TConsumer, TChannel> provider)
        {
            Trace.WriteLine(string.Format("InstanceChannelProvider<{0},{1}>", typeof(TConsumer).GetTypeName(),
                typeof(TChannel).GetTypeName()));

            return base.Visit(provider);
        }

        protected override Channel<T> Visit<T>(FilterChannel<T> channel)
        {
            Trace.WriteLine(string.Format("FilterChannel<{0}>", typeof(T).GetTypeName()));

            return base.Visit(channel);
        }

        protected override Channel<T> Visit<T>(InstanceChannel<T> channel)
        {
            Trace.WriteLine(string.Format("InstanceChannel<{0}>", typeof(T).GetTypeName()));

            return base.Visit(channel);
        }

        protected override Channel<T> Visit<T>(IntervalChannel<T> channel)
        {
            Trace.WriteLine(string.Format("IntervalChannel<{0}>, Interval = {1}", typeof(T).GetTypeName(),
                channel.Interval));

            return base.Visit(channel);
        }

        protected override Channel<T> Visit<T>(InterceptorChannel<T> channel)
        {
            Trace.WriteLine(string.Format("InterceptorChannel<{0}>", typeof(T).GetTypeName()));

            return base.Visit(channel);
        }

        protected override Channel<ICollection<T>> Visit<T, TKey>(DistinctChannel<T, TKey> channel)
        {
            Trace.WriteLine(string.Format("DistinctChannel<{0}>, Key = {1}", typeof(T).GetTypeName(),
                typeof(TKey).GetTypeName()));

            return base.Visit(channel);
        }

        protected override Channel<ICollection<T>> Visit<T>(LastChannel<T> channel)
        {
            Trace.WriteLine(string.Format("LastChannel<{0}>", typeof(T).GetTypeName()));

            return base.Visit(channel);
        }

        protected override Channel<T> Visit<T>(AsyncResultChannel<T> channel)
        {
            Trace.WriteLine(string.Format("AsyncResultChannel<{0}>, {1}", typeof(T).GetTypeName(),
                channel.IsCompleted ? "Complete" : "Pending"));

            return base.Visit(channel);
        }

        protected override Channel<T> Visit<T>(ShuntChannel<T> channel)
        {
            Trace.WriteLine(string.Format("ShuntChannel<{0}>", typeof(T).GetTypeName()));

            return base.Visit(channel);
        }

        protected override Channel<T> Visit<T>(ChannelAdapter<T> channel)
        {
            Trace.WriteLine(string.Format("ChannelAdapter<{0}>", typeof(T).GetTypeName()));

            return base.Visit(channel);
        }

        protected override Channel<T> Visit<T>(BroadcastChannel<T> channel)
        {
            Trace.WriteLine(string.Format("ChannelRouter<{0}>, {1} subscribers", typeof(T).GetTypeName(),
                channel.Listeners.Count()));

            return base.Visit(channel);
        }

        protected override Channel<TInput> Visit<TInput, TOutput>(ConvertChannel<TInput, TOutput> channel)
        {
            Trace.WriteLine(string.Format("ConvertChannel<{0}>, Output: {1}", typeof(TInput).GetTypeName(),
                typeof(TOutput).GetTypeName()));

            return base.Visit(channel);
        }

        protected override Channel<T> Visit<T>(Channel<T> channel)
        {
            Trace.WriteLine(string.Format("Channel<{0}>", typeof(T).Name));

            return base.Visit(channel);
        }

        protected override UntypedChannel Visit(UntypedChannel channel)
        {
            Trace.WriteLine(string.Format("UntypedChannel: {0}", channel.GetType().Name));

            return base.Visit(channel);
        }

        protected override UntypedChannel Visit<TFilter>(UntypedFilterChannel<TFilter> channel)
        {
            Trace.WriteLine(string.Format("UntypedFilterChannel<{0}>:", typeof(TFilter).GetTypeName()));

            return base.Visit(channel);
        }

        protected override UntypedChannel Visit(ChannelAdapter channel)
        {
            Trace.WriteLine("UntypedChannelAdapter");

            return base.Visit(channel);
        }

        protected override UntypedChannel Visit(BroadcastChannel channel)
        {
            Trace.WriteLine(string.Format("UntypedChannelRouter: {0} subscribers", channel.Listeners.Count()));

            return base.Visit(channel);
        }

        protected override UntypedChannel Visit<T>(TypedChannelAdapter<T> channel)
        {
            Trace.WriteLine(string.Format("TypedChannelAdapter<{0}>", typeof(T).Name));

            return base.Visit(channel);
        }

        protected override ChannelProvider<T> Visit<T>(ChannelProvider<T> provider)
        {
            Trace.WriteLine(string.Format("ChannelProvider<{0}>", typeof(T).Name));

            return base.Visit(provider);
        }

        protected override ChannelProvider<T> Visit<T>(DelegateChannelProvider<T> provider)
        {
            Trace.WriteLine(string.Format("DelegateChannelProvider<{0}>", typeof(T).Name));

            return base.Visit(provider);
        }

        protected override ChannelProvider<T> Visit<T, TKey>(KeyedChannelProvider<T, TKey> provider)
        {
            Trace.WriteLine(string.Format("KeyedChannelProvider<{0}>, Key = {1}", typeof(T).Name, typeof(TKey).Name));

            return base.Visit(provider);
        }

        protected override ChannelProvider<T> Visit<T>(ThreadStaticChannelProvider<T> provider)
        {
            Trace.WriteLine(string.Format("ThreadStaticChannelProvider<{0}>", typeof(T).Name));

            return base.Visit(provider);
        }

        protected override InterceptorFactory<T> Visit<T>(InterceptorFactory<T> factory)
        {
            Trace.WriteLine(string.Format("InterceptorFactory<{0}>", typeof(T).Name));

            return base.Visit(factory);
        }
    }
}