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


    public static class ExecuteFiberExtensions
    {
        /// <summary>
        /// Execute an Action on a Fiber
        /// </summary>
        /// <param name="fiber">The target Fiber</param>
        /// <param name="operation">The operation to execute</param>
        public static void Execute(this Fiber fiber, Action operation)
        {
            IList<Action> actions = new List<Action>(1);
            actions.Add(operation);
            var executor = new ActionListExecution(actions);

            fiber.Add(executor);
        }

        /// <summary>
        /// Execute a series of Actions on a Fiber
        /// </summary>
        /// <param name="fiber">The target Fiber</param>
        /// <param name="operations">The operations to execute</param>
        public static void Execute(this Fiber fiber, params Action[] operations)
        {
            var actions = new List<Action>(operations.Length);
            actions.AddRange(operations);
            var executor = new ActionListExecution(actions);

            fiber.Add(executor);
        }
    }
}