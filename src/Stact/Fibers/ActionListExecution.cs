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
    using System.Threading.Tasks;


    /// <summary>
    /// Wraps the execution of a list of Action into a Task
    /// </summary>
    public class ActionListExecution :
        Execution
    {
        readonly IList<Action> _actions;
        readonly NotifyActionsNotExecuted _notify;

        public ActionListExecution(IList<Action> actions)
            : this(actions, DefaultNotify)
        {
        }

        public ActionListExecution(IList<Action> actions, NotifyActionsNotExecuted notify)
        {
            _notify = notify;
            _actions = actions;
        }

        Task Execution.Execute(ExecutionContext executionContext)
        {
            int index = 0;
            try
            {
                for (; index < _actions.Count; index++)
                {
                    if (executionContext.CancellationToken.IsCancellationRequested)
                    {
                        _notify(_actions, index, _actions.Count - index);
                        return executionContext.Canceled();
                    }

                    _actions[index]();
                }

                return executionContext.Completed();
            }
            catch (Exception ex)
            {
                int next = index + 1;
                _notify(_actions, next, _actions.Count - next);

                return executionContext.Faulted(ex);
            }
        }

        static void DefaultNotify(IList<Action> actions, int index, int count)
        {
        }
    }
}