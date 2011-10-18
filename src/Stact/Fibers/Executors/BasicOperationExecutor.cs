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
namespace Stact.Executors
{
    using System;
    using System.Collections.Generic;
    using Internal;


    public class BasicOperationExecutor :
        OperationExecutor
    {
        bool _stopping;

        public void Execute(Action operation)
        {
            if (_stopping)
                return;

            operation();
        }

        public void Execute(IList<Action> operations, Action<IEnumerable<Action>> remaining)
        {
            for (int i = 0; i < operations.Count; i++)
            {
                if (_stopping)
                    break;

                operations[i]();
            }
        }

        public void Stop()
        {
            _stopping = true;
        }
    }
}