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


    /// <summary>
    ///   Configures the type of fiber to be used for handling messages
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    public interface FiberFactoryConfigurator<out T>
        where T : class
    {
        /// <summary>
        /// Execute synchronously on the calling thread
        /// </summary>
        T HandleOnCallingThread();

        /// <summary>
        /// Execute on the thread pool (using the TPL)
        /// </summary>
        T HandleOnThreadPool();

        /// <summary>
        /// Execute on a dedicated thread
        /// </summary>
        T HandleOnThread();

        /// <summary>
        /// Execute on an existing Fiber
        /// </summary>
        /// <param name = "fiber">The Fiber to schedule executions</param>
        T HandleOnFiber(Fiber fiber);

        /// <summary>
        /// Specify the FiberExceptionHandler to call when an exception occurs
        /// </summary>
        /// <param name="exceptionHandler">The exception handler</param>
        T SetExceptionHandler(FiberExceptionHandler exceptionHandler);

        /// <summary>
        /// Use a previously configured FiberFactory to create the Fiber
        /// </summary>
        /// <param name = "fiberFactory">The fiber factory to use</param>
        T UseFiberFactory(FiberFactory fiberFactory);

        /// <summary>
        /// Use a previously configured FiberFactoryEx to create the Fiber, allowing the configured
        /// exception handler to be passed to the factory.
        /// </summary>
        /// <param name = "fiberFactory">The FiberFactoryEx to use</param>
        T UseFiberFactory(FiberFactoryEx fiberFactory);

        /// <summary>
        /// Specify the Stop timeout for the Fiber
        /// </summary>
        /// <param name = "timeout"></param>
        T SetStopTimeout(TimeSpan timeout);
    }
}