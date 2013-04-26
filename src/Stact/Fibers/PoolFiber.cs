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
namespace Stact
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Executors;


    /// <summary>
    /// An Fiber that uses the .NET ThreadPool and QueueUserWorkItem to execute
    /// actions.
    /// </summary>
    public class PoolFiber :
        Fiber
    {
        readonly OperationExecutor _executor;
        readonly object _lock = new object();
        readonly IList<Executor> _empty = new List<Executor>();

        bool _executorQueued;
        IList<Executor> _operations = new List<Executor>();
        bool _shuttingDown;

        public PoolFiber()
            : this(new TryCatchOperationExecutor())
        {
        }

        public PoolFiber(OperationExecutor executor)
        {
            _executor = executor;
        }

        public void Add(Executor executor)
        {
            if (_shuttingDown)
                return;

            lock (_lock)
            {
                _operations.Add(executor);
                if (!_executorQueued)
                    QueueWorkItem();
            }
        }

        public bool Stop(TimeSpan timeout)
        {
            if (timeout == TimeSpan.Zero)
            {
                lock (_lock)
                {
                    _shuttingDown = true;
                    return _operations.Count == 0 && _executorQueued == false;
                }
            }

            DateTime waitUntil = DateTime.Now + timeout;

            lock (_lock)
            {
                _shuttingDown = true;
                Monitor.PulseAll(_lock);

                while (_operations.Count > 0 || _executorQueued)
                {
                    timeout = waitUntil - DateTime.Now;
                    if (timeout < TimeSpan.Zero)
                        throw new FiberException(
                            "Timeout expired waiting for all pending actions to complete during shutdown");

                    Monitor.Wait(_lock, timeout);
                }
            }

            return true;
        }

        public void Kill()
        {
            _shuttingDown = true;

            _executor.Stop();
        }

        public override string ToString()
        {
            return string.Format("{0} (Count: {1})", typeof(ThreadFiber).Name, _operations.Count);
        }

        void QueueWorkItem()
        {
            if (!ThreadPool.QueueUserWorkItem(x => Execute()))
                throw new FiberException("QueueUserWorkItem did not accept our execute method");

            _executorQueued = true;
        }

        bool Execute()
        {
            IList<Executor> operations = RemoveAll();

            _executor.Execute(operations, remaining =>
                {
                    lock (_lock)
                    {
                        int i = 0;
                        foreach (Executor action in remaining)
                            _operations.Insert(i++, action);
                    }
                });

            lock (_lock)
            {
                if (_operations.Count == 0)
                {
                    _executorQueued = false;

                    Monitor.PulseAll(_lock);
                }
                else
                    QueueWorkItem();
            }

            return true;
        }

        IList<Executor> RemoveAll()
        {
            lock (_lock)
            {
                if (_operations.Count == 0)
                    return _empty;

                IList<Executor> operations = _operations;

                _operations = new List<Executor>();

                return operations;
            }
        }
    }
}