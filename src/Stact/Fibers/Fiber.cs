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
namespace Stact
{
    using System;

    /// <summary>
    /// A Fiber is a concurrency unit in which operations are executed sequentially in order
    /// while retaining control of the execution context.
    /// </summary>
    public interface Fiber
    {
        /// <summary>
        /// Schedules an executor to run on the fiber. The execution occurs after all
        /// previous executions complete. If a previously scheduled execution faults, and the
        /// fault is unhandled, the fiber is killed, discarding any pending executors.
        /// </summary>
        /// <param name="execution">The executor</param>
        void Add(Execution execution);

        /// <summary>
        /// Stops the fiber, preventing any additional executors from being added. All scheduled
        /// executors are executed. If all executors do not complete before the timeout expires,
        /// an exception is thrown.
        /// </summary>
        /// <param name="timeout">The timeout before throwing an exception if executors have not all been executed</param>
        bool Stop(TimeSpan timeout);

        /// <summary>
        /// Kills the fiber, discarding any remaining executors, and aborting any currently executing executors.
        /// </summary>
        void Kill();
    }
}