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
    using Internal;
    using Magnum;
    using Magnum.Extensions;


    public class ThreadFiber :
        Fiber
    {
        readonly OperationExecutor _executor;
        readonly object _lock = new object();
        readonly Thread _thread;

        bool _isActive;
        IList<Action> _operations = new List<Action>();
        bool _shuttingDown;
        bool _stopping;

        public ThreadFiber()
            : this(new BasicOperationExecutor())
        {
        }

        public ThreadFiber(OperationExecutor executor)
        {
            _executor = executor;
            _thread = CreateThread();
            _thread.Start();
        }

        public void Add(Action operation)
        {
            if (_shuttingDown)
                return;
                // seems to be causing more problems that it solves
                // throw new FiberException("The fiber is no longer accepting actions");

            lock (_lock)
            {
                _operations.Add(operation);

                Monitor.PulseAll(_lock);
            }
        }

        public void Shutdown(TimeSpan timeout)
        {
            if (timeout == TimeSpan.Zero)
            {
                lock (_lock)
                {
                    _shuttingDown = true;
                    Monitor.PulseAll(_lock);
                }

                return;
            }

            DateTime waitUntil = SystemUtil.Now + timeout;

            lock (_lock)
            {
                _shuttingDown = true;

                Monitor.PulseAll(_lock);

                while (_operations.Count > 0 || _isActive)
                {
                    timeout = waitUntil - SystemUtil.Now;
                    if (timeout < TimeSpan.Zero)
                        throw new FiberException("Timeout expired waiting for all pending actions to complete during shutdown");

                    Monitor.Wait(_lock, timeout);
                }
            }

            _thread.Join(timeout);
        }

        public void Stop()
        {
            lock (_lock)
            {
                _shuttingDown = true;
                _stopping = true;
                _executor.Stop();

                Monitor.PulseAll(_lock);
            }
        }

        public override string ToString()
        {
            return "{0} (Count: {1}, Id: {2})".FormatWith(typeof(ThreadFiber).Name, _operations.Count, _thread.ManagedThreadId);
        }

        Thread CreateThread()
        {
            var thread = new Thread(Run);
            thread.Name = typeof(ThreadFiber).Name + "-" + thread.ManagedThreadId;
            thread.IsBackground = false;
            thread.Priority = ThreadPriority.Normal;

            return thread;
        }

        void Run()
        {
            _isActive = true;

            try
            {
                while (Execute())
                {
                }
            }
            catch
            {
            }

            _isActive = false;

            lock (_lock)
            {
                Monitor.PulseAll(_lock);
            }
        }

        bool Execute()
        {
            if (!WaitForActions())
                return false;

            IList<Action> operations = RemoveAll();
            if (operations == null)
                return false;

            _executor.Execute(operations, remaining =>
            {
                lock (_lock)
                {
                    int i = 0;
                    foreach (Action action in remaining)
                        _operations.Insert(i++, action);
                }
            });

            lock (_lock)
            {
                if (_operations.Count == 0)
                    Monitor.PulseAll(_lock);
            }

            return true;
        }

        bool WaitForActions()
        {
            lock (_lock)
            {
                while (_operations.Count == 0 && !_stopping && !_shuttingDown)
                    Monitor.Wait(_lock);

                if (_stopping)
                    return false;

                if (_shuttingDown)
                    return _operations.Count > 0;
            }

            return true;
        }

        IList<Action> RemoveAll()
        {
            lock (_lock)
            {
                IList<Action> operations = _operations;

                _operations = new List<Action>();

                return operations;
            }
        }
    }
}