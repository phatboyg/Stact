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


    public class TaskFiber :
        Fiber
    {
        readonly HandlerStack<FiberExceptionHandler> _exceptionHandlers;
        readonly ExecutionContext _executionContext;
        readonly CancellationTokenSource _kill;
        readonly object _lock;
        readonly CancellationTokenSource _shutdown;
        Task _task;

        public TaskFiber()
        {
            _lock = new object();
            _task = TaskUtil.Completed();
            _shutdown = new CancellationTokenSource();
            _kill = new CancellationTokenSource();

            _executionContext = new TaskFiberExecutionContext(_kill.Token);

            _exceptionHandlers = new HandlerStack<FiberExceptionHandler>(KillFiberExceptionHandler);
        }

        public TaskFiber(FiberExceptionHandler exceptionHandler)
            : this()
        {
            _exceptionHandlers.Push(exceptionHandler);
        }

        public void Add(Execution execution)
        {
            if (_shutdown.IsCancellationRequested)
                return;

            lock (_lock)
            {
                Task executeTask = Execute(_task, execution);

                Task faultTask = HandleFault(executeTask);

                _task = faultTask;
            }
        }

        public bool Stop(TimeSpan timeout)
        {
            _shutdown.Cancel();

            Task task;
            lock (_lock)
            {
                task = _task;
            }

            if (timeout == TimeSpan.Zero)
                return task.IsCompleted || task.IsCanceled;

            bool completed = task.Wait(timeout);
            if (completed)
                completed = task.IsCompleted;

            return completed;
        }

        public void Kill()
        {
            _shutdown.Cancel();
            _kill.Cancel();

            lock (_lock)
            {
                _task.ContinueWith(x => x.MarkObserved(), TaskContinuationOptions.NotOnRanToCompletion);
            }
        }

        Task Execute(Task task, Execution execution)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                    return TaskUtil.Faulted(task.Exception.InnerExceptions);

                if (task.IsCanceled || _kill.IsCancellationRequested)
                    return TaskUtil.Canceled();
            }

            return ExecuteAsync(task, execution);
        }


        Task ExecuteAsync(Task task, Execution execution)
        {
            var completion = new TaskCompletionSource<Task>();
            task.ContinueWith(innerTask =>
                {
                    if (innerTask.IsFaulted)
                        completion.TrySetException(innerTask.Exception.InnerExceptions);
                    else if (innerTask.IsCanceled || _kill.IsCancellationRequested)
                        completion.TrySetCanceled();
                    else
                    {
                        try
                        {
                            Task result = execution.Execute(_executionContext);

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

        Task HandleFault(Task task)
        {
            var source = new TaskCompletionSource<Task>();

            task.ContinueWith(innerTask =>
                {
                    if (innerTask.IsCanceled)
                        source.TrySetCanceled();
                    else
                        source.TrySetResult(TaskUtil.Completed());
                }, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

            task.ContinueWith(innerTask =>
                {
                    Exception exception = innerTask.Exception.InnerException;
                    try
                    {
                        innerTask.MarkObserved();

                        _exceptionHandlers.Enumerate(handlerEnumerator =>
                            {
                                NextExceptionHandler toNextHandler = null;
                                toNextHandler = ex =>
                                    {
                                        if (_kill.IsCancellationRequested)
                                        {
                                            source.SetCanceled();
                                            return;
                                        }

                                        if (handlerEnumerator.MoveNext())
                                        {
                                            FiberExceptionHandler nextHandler = handlerEnumerator.Current;
                                            nextHandler(ex, toNextHandler);

                                            source.TrySetResult(TaskUtil.Completed());
                                        }
                                        else
                                            source.TrySetException(exception);
                                    };

                                toNextHandler(exception);
                            });
                    }
                    catch (Exception ex)
                    {
                        source.TrySetException(ex);
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);

            return source.Task.FastUnwrap();
        }

        void KillFiberExceptionHandler(Exception exception, NextExceptionHandler next)
        {
            Kill();
        }
    }
}