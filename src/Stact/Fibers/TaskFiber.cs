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


    public delegate void FiberExceptionHandler(Exception exception, NextExceptionHandler next);


    public class TaskFiber :
        Fiber
    {
        readonly HandlerStack<FiberExceptionHandler> _exceptionHandlers;
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

            _exceptionHandlers = new HandlerStack<FiberExceptionHandler>(DefaultExceptionHandler);
        }

        public TaskFiber(FiberExceptionHandler exceptionHandler)
            : this()
        {
            _exceptionHandlers.Push(exceptionHandler);
        }

        public void Add(Executor executor)
        {
            if (_shutdown.IsCancellationRequested)
                return;

            TaskExecutor taskExecutor = new AsyncTaskExecutor(executor);

            lock (_lock)
            {
                Task executeTask = taskExecutor.Execute(_task, _kill.Token);

                Task faultTask = HandleFault(executeTask, _kill.Token);

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

        Task HandleFault(Task task, CancellationToken cancellationToken)
        {
            var source = new TaskCompletionSource<Task>();

            task.ContinueWith(innerTask =>
                {
                    if (innerTask.IsCanceled)
                        return source.TrySetCanceled();

                    return source.TrySetResult(TaskUtil.Completed());
                }, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

            task.ContinueWith(innerTask =>
                {
                    Exception exception = innerTask.Exception.InnerException;
                    Console.WriteLine("Handling faulted task {0}", exception);
                    try
                    {
                        innerTask.MarkObserved();

                        _exceptionHandlers.Enumerate(handlerEnumerator =>
                            {
                                Console.WriteLine("Enumerating...");

                                NextExceptionHandler toNextHandler = null;
                                toNextHandler = ex =>
                                    {
                                        if (cancellationToken.IsCancellationRequested)
                                        {
                                            source.SetCanceled();
                                            return;
                                        }

                                        if (handlerEnumerator.MoveNext())
                                        {
                                            Console.WriteLine("Moved next");

                                            FiberExceptionHandler nextHandler = handlerEnumerator.Current;
                                            if (nextHandler != null)
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

        void DefaultExceptionHandler(Exception exception, NextExceptionHandler next)
        {
            Console.WriteLine("Default exception handler: Kills Fiber");
            Kill();
        }
    }
}