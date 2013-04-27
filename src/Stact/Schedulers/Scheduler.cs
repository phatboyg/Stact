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


    public interface Scheduler
    {
        /// <summary>
        /// Schedules an operation to be executed after the special interval has elapsed
        /// </summary>
        /// <param name="interval">The duration of the interval</param>
        /// <param name="fiber">The fiber where the operation should be added</param>
        /// <param name="execution">The operation to execute</param>
        /// <returns>A ScheduledOperation reference</returns>
        ScheduledExecutionHandle Schedule(TimeSpan interval, Fiber fiber, Execution execution);

        /// <summary>
        /// Disables the scheduler, preventing any further scheduled operations from being executed
        /// </summary>
        /// <param name="timeout">The time period to wait for the scheduler to shutdown</param>
        void Stop(TimeSpan timeout);
    }
}