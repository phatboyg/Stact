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
namespace Stact.Schedulers
{
    using System;
    using System.Threading.Tasks;


    public class PeriodicScheduledExecution :
        ScheduledExecution,
        Execution
    {
        readonly Execution _execution;
        readonly Fiber _executionFiber;
        readonly TimeSpan _periodicInterval;
        readonly Scheduler _scheduler;
        bool _canceled;
        ScheduledExecutionHandle _handle;

        public PeriodicScheduledExecution(Scheduler scheduler, TimeSpan interval, TimeSpan periodicInterval,
            Fiber executionFiber,
            Execution execution)
        {
            _scheduler = scheduler;
            _periodicInterval = periodicInterval;
            _executionFiber = executionFiber;
            _execution = execution;

            _handle = scheduler.Schedule(interval, executionFiber, this);
        }

        public Task Execute(ExecutionContext executionContext)
        {
            if (executionContext.CancellationToken.IsCancellationRequested)
                return executionContext.Canceled();

            if (_canceled)
                return executionContext.Completed();

            Task task = _execution.Execute(executionContext);

            return task.ContinueWith(innerTask =>
                {
                    // the scheduler will invoke this on the scheduler fiber so no worries
                    _handle = _scheduler.Schedule(_periodicInterval, _executionFiber, this);
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        public DateTime ScheduledAt
        {
            get { return _handle.ScheduledAt; }
        }

        public void Cancel()
        {
            _canceled = true;
            _handle.Cancel();
        }

        public void Execute()
        {
            if (_canceled)
                return;

            _executionFiber.Add(this);
        }
    }
}