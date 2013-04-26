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
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Tasks;


    public class AsyncTaskExecutor :
        TaskExecutor
    {
        readonly Executor _executor;

        public AsyncTaskExecutor(Executor executor)
        {
            _executor = executor;
        }

        Task TaskExecutor.Execute(Task task, CancellationToken cancellationToken)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                    return TaskUtil.Faulted(task.Exception.InnerExceptions);

                if (task.IsCanceled || cancellationToken.IsCancellationRequested)
                    return TaskUtil.Canceled();
            }

            return ExecuteAsync(task, cancellationToken);
        }


        Task ExecuteAsync(Task task, CancellationToken cancellationToken)
        {
            var completion = new TaskCompletionSource<Task>();
            task.ContinueWith(innerTask =>
                {
                    if (innerTask.IsFaulted)
                        completion.TrySetException(innerTask.Exception.InnerExceptions);
                    else if (innerTask.IsCanceled || cancellationToken.IsCancellationRequested)
                        completion.TrySetCanceled();
                    else
                    {
                        try
                        {
                            Task result = _executor.Execute(cancellationToken);

                            completion.TrySetResult(result);
                        }
                        catch (Exception ex)
                        {
                            completion.TrySetException(ex);
                        }
                    }
                });

            return completion.Task.FastUnwrap();
        }
    }
}