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
namespace Magnum.Channels.Configuration.Internal
{
	using System;
	using Extensions;
	using Fibers;


	public class FiberModelConfigurator<T>
		where T : class
	{
		FiberFactory _fiberFactory;
		TimeSpan _shutdownTimeout = 1.Minutes();

		public FiberModelConfigurator()
		{
			ExecuteOnThreadPoolFiber();
		}

		public T ExecuteOnFiber(Fiber fiber)
		{
			_fiberFactory = () => fiber;

			return this as T;
		}

		public T ExecuteOnThread()
		{
			_fiberFactory = () => new ThreadFiber();

			return this as T;
		}

		public T ExecuteOnProducerThread()
		{
			_fiberFactory = () => new SynchronousFiber();

			return this as T;
		}

		public T ExecuteOnThreadPoolFiber()
		{
			_fiberFactory = () => new ThreadPoolFiber();

			return this as T;
		}

		public T UseFiberFactory(FiberFactory fiberFactory)
		{
			_fiberFactory = fiberFactory;

			return this as T;
		}

		public T SetShutdownTimeout(TimeSpan timeout)
		{
			_shutdownTimeout = timeout;

			return this as T;
		}

		protected Fiber GetConfiguredFiber(ChannelConfiguratorConnection connection)
		{
			Fiber fiber = _fiberFactory();
			connection.AddDisposable(fiber.GetShutdownDisposable(_shutdownTimeout));

			return fiber;
		}

		protected Fiber GetConfiguredFiber<TChannel>(ChannelConfiguratorConnection<TChannel> connection)
		{
			Fiber fiber = _fiberFactory();
			connection.AddDisposable(fiber.GetShutdownDisposable(_shutdownTimeout));

			return fiber;
		}
	}
}