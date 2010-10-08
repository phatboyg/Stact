// Copyright 2010 Chris Patterson
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
namespace Stact.Configuration.Internal
{
	using System;
	using System.Collections.Generic;


	public class IntervalChannelConfiguratorImpl<TChannel> :
		SchedulerFactoryConfiguratorImpl<IntervalChannelConfigurator<TChannel>>,
		IntervalChannelConfigurator<TChannel>,
		ChannelConfigurator<TChannel>
	{
		ChannelConfigurator<ICollection<TChannel>> _configurator;
		TimeSpan _interval;

		public IntervalChannelConfiguratorImpl(TimeSpan interval)
		{
			_interval = interval;

			UseTimerScheduler();
			HandleOnPoolFiber();
		}

		public void Configure(ChannelConfiguratorConnection<TChannel> connection)
		{
			Fiber fiber = this.GetFiberUsingConfiguredFactory(connection);
			Scheduler scheduler = GetSchedulerUsingConfiguredFactory(connection);

			_configurator.Configure(new IntervalChannelConfiguratorConnection(connection, fiber, scheduler, _interval));
		}

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException(typeof(TChannel), "No channel configurator was setup");

			_configurator.ValidateConfiguration();

			ValidateFiberFactoryConfiguration();
			ValidateSchedulerFactoryConfiguration();
		}

		public void SetChannelConfigurator(ChannelConfigurator<ICollection<TChannel>> configurator)
		{
			_configurator = configurator;
		}


		class IntervalChannelConfiguratorConnection :
			ChannelConfiguratorConnection<ICollection<TChannel>>
		{
			readonly ChannelConfiguratorConnection<TChannel> _connection;
			readonly Fiber _fiber;
			readonly TimeSpan _interval;
			readonly Scheduler _scheduler;

			public IntervalChannelConfiguratorConnection(ChannelConfiguratorConnection<TChannel> connection,
			                                             Fiber fiber,
			                                             Scheduler scheduler,
			                                             TimeSpan interval)
			{
				_connection = connection;
				_fiber = fiber;
				_scheduler = scheduler;
				_interval = interval;
			}

			public void AddChannel(Fiber fiber, Func<Fiber, Channel<ICollection<TChannel>>> channelFactory)
			{
				Channel<ICollection<TChannel>> channel = channelFactory(fiber);

				_connection.AddChannel(fiber, x => new IntervalChannel<TChannel>(_fiber, _scheduler, _interval, channel));
			}

			public void AddChannel<T>(Fiber fiber, Func<Fiber, Channel<T>> channelFactory)
			{
				throw new NotImplementedException("No way to buffer untyped messages at this point");
			}

			public void AddDisposable(IDisposable disposable)
			{
				_connection.AddDisposable(disposable);
			}
		}
	}
}