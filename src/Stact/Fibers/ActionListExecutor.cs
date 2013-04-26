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
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Tasks;


    public class ActionListExecutor :
        Executor
    {
        readonly IList<Action> _actions;
        readonly NotifyActionsNotExecuted _notify = (a, b, c) => { };

        public ActionListExecutor(IList<Action> actions)
        {
            _actions = actions;
        }

        Task Executor.Execute(CancellationToken cancellationToken)
        {
            int index = 0;
            try
            {
                for (; index < _actions.Count; index++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return TaskUtil.Canceled();

                    _actions[index]();
                }

                return TaskUtil.Completed();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception");

                int next = index + 1;
                _notify(_actions, next, _actions.Count - next);

                return TaskUtil.Faulted(ex);
            }
        }
    }
}