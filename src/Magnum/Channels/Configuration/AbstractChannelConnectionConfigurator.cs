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
	using System.Threading;
	using Fibers;

	public class AbstractChannelConnectionConfigurator<TChannel> :
		ChannelConnectionConfigurator<TChannel>
	{
		private FiberFactory _fiberFactory = ThreadPoolFiberProvider;
		private Func<Scheduler> _schedulerProvider = TimerSchedulerProvider;
		private SynchronizationContext _synchronizationContext;

		public AbstractChannelConnectionConfigurator()
		{
			ConsumerProvider = NoConsumerProviderConfigured;
		}

		protected AbstractChannelConnectionConfigurator(Channel<TChannel> channel)
		{
			ConsumerProvider = () => channel;
		}

		private Func<Channel<TChannel>> ConsumerProvider { get; set; }

		public ConsumerConfigurator<TConsumer, TChannel> UsingInstance<TConsumer>(ChannelAccessor<TConsumer, TChannel> channelAccessor)
		{
			var configurator = new ConsumerConfiguratorImpl<TConsumer, TChannel>(channelAccessor);

			ConsumerProvider = () => new InstanceChannel<TChannel>(configurator.GetChannelProvider());

			return configurator;
		}

		public ChannelConnectionConfigurator<TChannel> UsingSelectiveConsumer(SelectiveConsumer<TChannel> selectiveConsumer)
		{
			ConsumerProvider = () => new SelectiveConsumerChannel<TChannel>(_fiberFactory(), selectiveConsumer);

			return this;
		}

		public ChannelConnectionConfigurator<TChannel> UsingConsumer(Consumer<TChannel> consumer)
		{
			ConsumerProvider = () => new ConsumerChannel<TChannel>(_fiberFactory(), consumer);

			return this;
		}

		public ChannelConnectionConfigurator<ICollection<TChannel>> Every(TimeSpan interval)
		{
			var configurator = new IntervalChannelConnectionConfigurator<TChannel>();

			ConsumerProvider = () => new IntervalChannel<TChannel>(_fiberFactory(), _schedulerProvider(), interval, configurator.ConsumerProvider());

			return configurator;
		}

		public ChannelConnectionConfigurator<IDictionary<TKey, TChannel>> Every<TKey>(TimeSpan interval, KeyAccessor<TChannel, TKey> keyAccessor)
		{
			var configurator = new DistinctIntervalChannelConnectionConfigurator<TChannel, TKey>();

			ConsumerProvider = () => new DistinctIntervalChannel<TChannel, TKey>(_fiberFactory(), _schedulerProvider(), interval, keyAccessor, configurator.ConsumerProvider());

			return configurator;
		}

		protected Channel<TChannel> GetConsumer()
		{
			Channel<TChannel> channel = AddSynchronizationIfRequired(ConsumerProvider);

			return channel;
		}

		private Channel<TChannel> AddSynchronizationIfRequired(Func<Channel<TChannel>> provider)
		{
			// TODO maybe start building out channels from the outside in (intercepter style) to ensure fibre model is matched
			_synchronizationContext = _synchronizationContext ?? SynchronizationContext.Current;

			if (_synchronizationContext == null)
				return provider();

			Fiber fiber = _fiberFactory();
			_fiberFactory = () => new SynchronousFiber();

			return new SynchronizedChannel<TChannel>(fiber, provider(), _synchronizationContext);
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