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
    using System.Collections.Generic;


    public interface Fiber
    {
        /// <summary>
        /// Adds an operation at the tail of the list of operations for the fiber
        /// </summary>
        /// <param name="executor">The operation to add</param>
        void Add(Executor executor);

        /// <summary>
        /// Runs all remaining actions, waiting until all actions have been executed or until the
        /// timeout expires. If the timeout expires, an exception is thrown.
        /// </summary>
        /// <param name="timeout">The time to wait for all pending actions to be executed before throwing an exception</param>
        bool Stop(TimeSpan timeout);

        /// <summary>
        /// Kills the fiber, discards any remaining actions, and prevents new actions from being added
        /// May also abort the thread currently executing, forcing it to terminate any in-progress operations
        /// </summary>
        void Kill();
    }


    public static class Fiberieoiuwer
    {
    }
}