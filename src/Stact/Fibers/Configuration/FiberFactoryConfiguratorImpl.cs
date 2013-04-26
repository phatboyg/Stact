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
namespace Stact.Configuration
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using Executors;


    public abstract class FiberFactoryConfiguratorImpl<T> :
        FiberFactoryConfigurator<T>,
        Configurator
        where T : class
    {
        readonly Func<OperationExecutor> _executorFactory;
        FiberExceptionHandler _exceptionHandler = DefaultExceptionHandler;
        FiberFactoryEx _fiberFactory;
        TimeSpan _stopTimeout = TimeSpan.FromMinutes(1);

        protected FiberFactoryConfiguratorImpl()
        {
            HandleOnThreadPool();
            _executorFactory = () => new TryCatchOperationExecutor();
        }

        public TimeSpan StopTimeout
        {
            get { return _stopTimeout; }
        }

        public virtual IEnumerable<ValidateConfigurationResult> ValidateConfiguration()
        {
            if (_fiberFactory == null)
                yield return this.Failure("FiberFactory", "must be specified");
        }

        public T HandleOnCallingThread()
        {
            _fiberFactory = executor => new SynchronousFiber(_executorFactory());

            return this as T;
        }

        public T HandleOnThreadPool()
        {
            _fiberFactory = exceptionHandler => new TaskFiber(exceptionHandler);

            return this as T;
        }

        public T HandleOnFiber(Fiber fiber)
        {
            _fiberFactory = executor => fiber;

            return this as T;
        }

        public T HandleOnThread()
        {
            _fiberFactory = executor => new ThreadFiber(_executorFactory());

            return this as T;
        }

        public T SetExceptionHandler(FiberExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;

            return this as T;
        }

        public T UseFiberFactory(FiberFactory fiberFactory)
        {
            _fiberFactory = executor => fiberFactory();

            return this as T;
        }

        public T UseFiberFactory(FiberFactoryEx fiberFactory)
        {
            _fiberFactory = fiberFactory;

            return this as T;
        }

        public T SetStopTimeout(TimeSpan timeout)
        {
            _stopTimeout = timeout;

            return this as T;
        }

        static void DefaultExceptionHandler(Exception exception, NextExceptionHandler next)
        {
            throw new FiberException("An exception occurred", exception);
        }

        public FiberFactory GetConfiguredFiberFactory()
        {
            return () => _fiberFactory(_exceptionHandler);
        }

        protected FiberFactoryEx GetConfiguredFiberFactoryEx()
        {
            return _fiberFactory;
        }

        protected Fiber GetConfiguredFiber(ConnectionBuilder builder)
        {
            Fiber fiber = GetConfiguredFiberFactory()();

            builder.AddDisposable(fiber.StopOnDispose(_stopTimeout));

            return fiber;
        }

        public Fiber GetConfiguredFiber<TChannel>(ConnectionBuilder<TChannel> builder)
        {
            Fiber fiber = GetConfiguredFiberFactory()();

            builder.AddDisposable(fiber.StopOnDispose(_stopTimeout));

            return fiber;
        }

        protected Fiber GetConfiguredFiber<TChannel>(ChannelBuilder<TChannel> builder)
        {
            Fiber fiber = GetConfiguredFiberFactory()();

            builder.AddDisposable(fiber.StopOnDispose(_stopTimeout));

            return fiber;
        }
    }
}