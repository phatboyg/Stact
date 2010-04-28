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
	using Fibers;

	public class AbstractChannelSubscriptionConfigurator<TChannel> :
		ChannelSubscriptionConfigurator<TChannel>
	{
		private FiberProvider _fiberProvider = ThreadPoolFiberProvider;
		private Func<Scheduler> _schedulerProvider = TimerSchedulerProvider;

		public AbstractChannelSubscriptionConfigurator()
		{
			ConsumerProvider = NoConsumerProviderConfigured;
		}

		protected AbstractChannelSubscriptionConfigurator(Channel<TChannel> channel)
		{
			ConsumerProvider = () => channel;
		}

		protected Func<Channel<TChannel>> ConsumerProvider { get; private set; }

		public ConsumerConfigurator<TConsumer, TChannel> Using<TConsumer>(ChannelAccessor<TConsumer, TChannel> channelAccessor)
		{
			var configurator = new ConsumerConfiguratorImpl<TConsumer, TChannel>(channelAccessor);

			ConsumerProvider = () => configurator.GetChannel();

			return configurator;
		}

		public ChannelSubscriptionConfigurator<TChannel> Using(SelectiveConsumer<TChannel> selectiveConsumer)
		{
			ConsumerProvider = () => new SelectiveConsumerChannel<TChannel>(_fiberProvider(), selectiveConsumer);

			return this;
		}

		public ChannelSubscriptionConfigurator<TChannel> Using(Consumer<TChannel> consumer)
		{
			ConsumerProvider = () => new ConsumerChannel<TChannel>(_fiberProvider(), consumer);

			return this;
		}

		public ChannelSubscriptionConfigurator<ICollection<TChannel>> Every(TimeSpan interval)
		{
			var configurator = new IntervalChannelSubscriptionConfigurator<TChannel>();

			ConsumerProvider = () => new IntervalChannel<TChannel>(_fiberProvider(), _schedulerProvider(), interval, configurator.ConsumerProvider());

			return configurator;
		}

		public ChannelSubscriptionConfigurator<IDictionary<TKey,TChannel>> Every<TKey>(TimeSpan interval, KeyAccessor<TChannel, TKey> keyAccessor)
		{
			var configurator = new DistinctIntervalChannelSubscriptionConfigurator<TChannel, TKey>();

			ConsumerProvider = () => new DistinctIntervalChannel<TChannel, TKey>(_fiberProvider(), _schedulerProvider(), interval, keyAccessor, configurator.ConsumerProvider());

			return configurator;
		}

		private static Fiber ThreadPoolFiberProvider()
		{
			return new ThreadPoolFiber();
		}

		private static Scheduler TimerSchedulerProvider()
		{
			return new TimerScheduler(new ThreadPoolFiber());
		}

		private static Channel<TChannel> NoConsumerProviderConfigured()
		{
			throw new InvalidOperationException("A consumer implementation was not specified: " + typeof (TChannel).Name);
		}
	}
}