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
namespace Stact.Configuration
{
	using System;
	using Builders;
	using Magnum.Extensions;


	public class FiberFactoryConfiguratorImpl<T> :
		FiberFactoryConfigurator<T>
		where T : class
	{
		FiberFactory _fiberFactory;
		TimeSpan _shutdownTimeout = 1.Minutes();

		protected FiberFactoryConfiguratorImpl()
		{
			HandleOnPoolFiber();
		}

		public TimeSpan ShutdownTimeout
		{
			get { return _shutdownTimeout; }
		}

		public T HandleOnCallingThread()
		{
			_fiberFactory = () => new SynchronousFiber();

			return this as T;
		}

		public T HandleOnPoolFiber()
		{
			_fiberFactory = () => new PoolFiber();

			return this as T;
		}

		public T HandleOnFiber(Fiber fiber)
		{
			_fiberFactory = () => fiber;

			return this as T;
		}

		public T HandleOnThreadFiber()
		{
			_fiberFactory = () => new ThreadFiber();

			return this as T;
		}

		public T UseFiberFactory(FiberFactory fiberFactory)
		{
			_fiberFactory = fiberFactory;

			return this as T;
		}

		public T UseShutdownTimeout(TimeSpan timeout)
		{
			_shutdownTimeout = timeout;

			return this as T;
		}

		protected void ValidateFiberFactoryConfiguration()
		{
			if (_fiberFactory == null)
				throw new FiberException("No fiber configuration was specified");
		}

		public FiberFactory GetConfiguredFiberFactory()
		{
			return _fiberFactory;
		}

		public Fiber GetConfiguredFiber(ConnectionBuilder builder)
		{
			Fiber fiber = _fiberFactory();

			builder.AddDisposable(fiber.ShutdownOnDispose(_shutdownTimeout));

			return fiber;
		}

		public Fiber GetConfiguredFiber<TChannel>(ConnectionBuilder<TChannel> builder)
		{
			Fiber fiber = _fiberFactory();

			builder.AddDisposable(fiber.ShutdownOnDispose(_shutdownTimeout));

			return fiber;
		}

		public Fiber GetConfiguredFiber<TChannel>(ChannelBuilder<TChannel> builder)
		{
			Fiber fiber = _fiberFactory();

			builder.AddDisposable(fiber.ShutdownOnDispose(_shutdownTimeout));

			return fiber;
		}
	}
}