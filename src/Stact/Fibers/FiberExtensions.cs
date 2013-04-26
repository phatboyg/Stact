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
    using Internal;


    public static class FiberExtensions
    {
        /// <summary>
        /// Returns a disposable object that will stop the fiber
        /// </summary>
        /// <param name="fiber">The fiber to stop</param>
        /// <param name="timeout">The time to wait for the fiber to stop</param>
        /// <returns>The disposable object reference</returns>
        public static IDisposable StopOnDispose(this Fiber fiber, TimeSpan timeout)
        {
            return new StopFiberOnDispose(fiber, timeout);
        }
    }
}