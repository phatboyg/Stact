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
namespace Stact.Configuration.Builders
{
	using System;
	using System.Collections.Generic;


	public class IntervalChannelBuilder<TChannel> :
		ChannelBuilder<ICollection<TChannel>>
	{
		readonly ChannelBuilder<TChannel> _builder;
		readonly FiberFactory _fiberFactory;
		readonly TimeSpan _interval;
		readonly SchedulerFactory _schedulerFactory;

		public IntervalChannelBuilder(ChannelBuilder<TChannel> builder, FiberFactory fiberFactory,
		                              SchedulerFactory schedulerFactory, TimeSpan interval)
		{
			_builder = builder;
			_interval = interval;
			_schedulerFactory = schedulerFactory;
			_fiberFactory = fiberFactory;
		}

		public void AddChannel(Fiber fiber, Func<Fiber, Channel<ICollection<TChannel>>> channelFactory)
		{
			_builder.AddChannel(fiber, x =>
				{
					Channel<ICollection<TChannel>> channel = channelFactory(fiber);

					Fiber intervalFiber = _fiberFactory();
					Scheduler scheduler = _schedulerFactory();

					return new IntervalChannel<TChannel>(intervalFiber, scheduler, _interval, channel);
				});
		}


		public void AddDisposable(IDisposable disposable)
		{
			_builder.AddDisposable(disposable);
		}
	}
}