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
namespace Stact.Channels.Configuration.Internal
{
	using System;
	using Fibers;
	using Fibers.Configuration;


	public class IntervalModelConfigurator<T> :
		FiberConfiguratorImpl<T>
		where T : class
	{
		protected TimeSpan _interval;
		protected Func<Scheduler> _schedulerFactory;

		public T UsePrivateScheduler()
		{
			_schedulerFactory = () => new TimerScheduler(new ThreadPoolFiber());

			return this as T;
		}

		public T UseScheduler(Scheduler scheduler)
		{
			_schedulerFactory = () => scheduler;

			return this as T;
		}

		public T WithSchedulerFactory(Func<Scheduler> schedulerFactory)
		{
			_schedulerFactory = schedulerFactory;

			return this as T;
		}
	}
}