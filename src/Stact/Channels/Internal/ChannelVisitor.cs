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
namespace Stact.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internals.Extensions;


    public class ChannelVisitor
    {
        interface UntypedRedirector
        {
            UntypedChannel Visit(ChannelVisitor visitor);
        }

        interface TypedRedirector<T>
        {
            Channel<T> Visit(ChannelVisitor visitor);
        }

        class TypedChannelAdapterRedirector<T> :
            UntypedRedirector
        {
            readonly TypedChannelAdapter<T> _channel;

            public TypedChannelAdapterRedirector(UntypedChannel channel)
            {
                _channel = channel as TypedChannelAdapter<T>;
            }

            public UntypedChannel Visit(ChannelVisitor visitor)
            {
                return visitor.Visit(_channel);
            }
        }        
        
        class ConvertChannelRedirector<T,TOutput> :
            TypedRedirector<T>
        {
            readonly ConvertChannel<T,TOutput> _channel;

            public ConvertChannelRedirector(Channel<T> channel)
            {
                _channel = channel as ConvertChannel<T, TOutput>;
            }

            public Channel<T> Visit(ChannelVisitor visitor)
            {
                return visitor.Visit(_channel);
            }
        }

        class LastChannelRedirector<T> :
            TypedRedirector<ICollection<T>>
        {
            readonly LastChannel<T> _channel;

            public LastChannelRedirector(Channel<ICollection<T>> channel)
            {
                _channel = channel as LastChannel<T>;
            }

            public Channel<ICollection<T>> Visit(ChannelVisitor visitor)
            {
                return visitor.Visit(_channel);
            }
        }

        class DistinctChannelRedirector<T,TOutput> :
            TypedRedirector<ICollection<T>>
        {
            readonly DistinctChannel<T,TOutput> _channel;

            public DistinctChannelRedirector(Channel<ICollection<T>> channel)
            {
                _channel = channel as DistinctChannel<T, TOutput>;
            }

            public Channel<ICollection<T>> Visit(ChannelVisitor visitor)
            {
                return visitor.Visit(_channel);
            }
        }
        protected virtual Channel<T> Visit<T>(Channel<T> channel)
        {
            if (channel is ShuntChannel<T>)
                return Visit((ShuntChannel<T>)channel);
            if (channel is ConsumerChannel<T>)
                return Visit((ConsumerChannel<T>)channel);
            if (channel is SelectiveConsumerChannel<T>)
                return Visit((SelectiveConsumerChannel<T>)channel);
            if (channel is FilterChannel<T>)
                return Visit((FilterChannel<T>)channel);
            if (channel is ChannelAdapter<T>)
                return Visit((ChannelAdapter<T>)channel);
            if (channel is InstanceChannel<T>)
                return Visit((InstanceChannel<T>)channel);
            if (channel is IntervalChannel<T>)
                return Visit((IntervalChannel<T>)channel);
            if (channel is InterceptorChannel<T>)
                return Visit((InterceptorChannel<T>)channel);
            if (channel is SynchronizedChannel<T>)
                return Visit((SynchronizedChannel<T>)channel);
            if (channel is AsyncResultChannel<T>)
                return Visit((AsyncResultChannel<T>)channel);
            if (channel is BroadcastChannel<T>)
                return Visit((BroadcastChannel<T>)channel);
            if (channel.GetType().ClosesType(typeof(LastChannel<>)))
            {
                var closingArguments = channel.GetType().GetClosingArguments(typeof(LastChannel<>));
                var redirectorType = typeof(LastChannelRedirector<>).MakeGenericType(closingArguments.ToArray());
                var redirector = (TypedRedirector<T>)Activator.CreateInstance(redirectorType, channel);
                return redirector.Visit(this);
            }            if (channel.GetType().ClosesType(typeof(ConvertChannel<,>)))
            {
                var closingArguments = channel.GetType().GetClosingArguments(typeof(ConvertChannel<,>));
                var redirectorType = typeof(ConvertChannelRedirector<,>).MakeGenericType(closingArguments.ToArray());
                var redirector = (TypedRedirector<T>)Activator.CreateInstance(redirectorType, channel);
                return redirector.Visit(this);
            }
            if (channel.GetType().ClosesType(typeof(DistinctChannel<,>)))
            {
                var closingArguments = channel.GetType().GetClosingArguments(typeof(DistinctChannel<,>));
                var redirectorType = typeof(DistinctChannelRedirector<,>).MakeGenericType(closingArguments.ToArray());
                var redirector = (TypedRedirector<T>)Activator.CreateInstance(redirectorType, channel);
                return redirector.Visit(this);
            }

            throw new ArgumentException("Unknown channel type: " + channel.GetType().Name);
        }

        protected virtual UntypedChannel Visit(UntypedChannel channel)
        {
            if (channel is ShuntChannel)
                return Visit((ShuntChannel)channel);
            if (channel is BroadcastChannel)
                return Visit((BroadcastChannel)channel);
            if (channel is ChannelAdapter)
                return Visit((ChannelAdapter)channel);
            if (channel is SynchronizedChannel)
                return Visit((SynchronizedChannel)channel);
            if (channel is AsyncResultChannel)
                return Visit((AsyncResultChannel)channel);
            if (channel.GetType().ClosesType(typeof(TypedChannelAdapter<>)))
            {
                var closingArguments = channel.GetType().GetClosingArguments(typeof(TypedChannelAdapter<>));
                var redirectorType =typeof(TypedChannelAdapterRedirector<>).MakeGenericType(closingArguments.Single());
                var redirector = (UntypedRedirector)Activator.CreateInstance(redirectorType, channel);
                return redirector.Visit(this);
            }

            throw new ArgumentException("Unknown channel type: " + channel.GetType().Name);
        }

        protected virtual UntypedChannel Visit(SynchronizedChannel channel)
        {
            return channel;
        }

        protected virtual UntypedChannel Visit(AsyncResultChannel channel)
        {
            return channel;
        }

        protected virtual Channel<T> Visit<T>(SelectiveConsumerChannel<T> channel)
        {
            return channel;
        }

        protected virtual Channel<T> Visit<T>(ConsumerChannel<T> channel)
        {
            return channel;
        }

        protected virtual Channel<T> Visit<T>(ShuntChannel<T> channel)
        {
            return channel;
        }

        protected virtual Channel<T> Visit<T>(FilterChannel<T> channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual Channel<T> Visit<T>(ChannelAdapter<T> channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual Channel<T> Visit<T>(InstanceChannel<T> channel)
        {
            Visit(channel.Provider);

            return channel;
        }

        protected virtual Channel<T> Visit<T>(IntervalChannel<T> channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual Channel<T> Visit<T>(InterceptorChannel<T> channel)
        {
            Visit(channel.Output);

            Visit(channel.InterceptorFactory);

            return channel;
        }

        protected virtual Channel<T> Visit<T>(SynchronizedChannel<T> channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual Channel<ICollection<T>> Visit<T, TKey>(DistinctChannel<T, TKey> channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual Channel<ICollection<T>> Visit<T>(LastChannel<T> channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual Channel Visit<T>(AsyncResultChannel channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual Channel<T> Visit<T>(AsyncResultChannel<T> channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual Channel<T> Visit<T>(BroadcastChannel<T> channel)
        {
            foreach (var listener in channel.Listeners)
                Visit(listener);

            return channel;
        }

        protected virtual Channel<TInput> Visit<TInput, TOutput>(ConvertChannel<TInput, TOutput> channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual UntypedChannel Visit(ChannelAdapter channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual UntypedChannel Visit<TFilter>(UntypedFilterChannel<TFilter> channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual UntypedChannel Visit(ShuntChannel channel)
        {
            return channel;
        }

        protected virtual UntypedChannel Visit(BroadcastChannel channel)
        {
            foreach (UntypedChannel listener in channel.Listeners)
                Visit(listener);

            return channel;
        }

        protected virtual UntypedChannel Visit<T>(TypedChannelAdapter<T> channel)
        {
            Visit(channel.Output);

            return channel;
        }

        protected virtual ChannelProvider<T> Visit<T>(ChannelProvider<T> provider)
        {
            return provider;
        }

        protected virtual ChannelProvider<TChannel> Visit<TConsumer, TChannel>(
            InstanceChannelProvider<TConsumer, TChannel> provider)
            where TConsumer : class
        {
            return provider;
        }

        protected virtual ChannelProvider<T> Visit<T>(DelegateChannelProvider<T> provider)
        {
            return provider;
        }

        protected virtual ChannelProvider<T> Visit<T, TKey>(KeyedChannelProvider<T, TKey> provider)
        {
            //Visit(provider.ChannelProvider);

            return provider;
        }

        protected virtual ChannelProvider<T> Visit<T>(ThreadStaticChannelProvider<T> provider)
        {
            Visit(provider.InstanceProvider);

            return provider;
        }

        protected virtual InterceptorFactory<T> Visit<T>(InterceptorFactory<T> factory)
        {
            return factory;
        }
    }
}