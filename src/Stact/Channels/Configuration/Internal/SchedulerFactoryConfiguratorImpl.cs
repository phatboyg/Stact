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
	using Stact.Internal;


	public class SchedulerFactoryConfiguratorImpl<T> :
		FiberFactoryConfiguratorImpl<T>,
		SchedulerFactoryConfigurator<T>
		where T : class
	{
		bool _owned;
		SchedulerFactory _schedulerFactory;

		public T UseTimerScheduler()
		{
			_schedulerFactory = () => new TimerScheduler(new PoolFiber());
			_owned = true;

			return this as T;
		}

		public T UseScheduler(Scheduler scheduler)
		{
			_schedulerFactory = () => scheduler;
			_owned = false;

			return this as T;
		}

		public T UseSchedulerFactory(SchedulerFactory schedulerFactory)
		{
			_schedulerFactory = schedulerFactory;
			_owned = false;

			return this as T;
		}

		protected void ValidateSchedulerFactoryConfiguration()
		{
			if (_schedulerFactory == null)
				throw new SchedulerException("A SchedulerFactory was not configured");
		}

		public Scheduler GetSchedulerUsingConfiguredFactory<TChannel>(ConnectionBuilder<TChannel> connection)
		{
			Scheduler scheduler = _schedulerFactory();

			if (_owned)
				connection.AddDisposable(new DisposeCallback(() => scheduler.Stop(ShutdownTimeout)));

			return scheduler;
		}
	}
}