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
    using Executors;


    public class ThreadFiber :
        Fiber
    {
        readonly OperationExecutor _executor;
        readonly object _lock = new object();
        readonly Thread _thread;

        bool _isActive;
        bool _killed;
        IList<Execution> _operations = new List<Execution>();
        bool _stopped;

        public ThreadFiber()
            : this(new TryCatchOperationExecutor())
        {
        }

        public ThreadFiber(OperationExecutor executor)
        {
            _executor = executor;
            _thread = CreateThread();
            _thread.Start();
        }

        public void Add(Execution execution)
        {
            if (_stopped)
                return;
            // seems to be causing more problems that it solves
            // throw new FiberException("The fiber is no longer accepting actions");

            lock (_lock)
            {
                _operations.Add(execution);

                Monitor.PulseAll(_lock);
            }
        }

        public bool Stop(TimeSpan timeout)
        {
            if (timeout == TimeSpan.Zero)
            {
                lock (_lock)
                {
                    _stopped = true;
                    Monitor.PulseAll(_lock);

                    return _operations.Count == 0 && _isActive == false;
                }
            }

            DateTime waitUntil = DateTime.Now + timeout;

            lock (_lock)
            {
                _stopped = true;

                Monitor.PulseAll(_lock);

                while (_operations.Count > 0 || _isActive)
                {
                    timeout = waitUntil - DateTime.Now;
                    if (timeout < TimeSpan.Zero)
                    {
                        throw new FiberException(
                            "Timeout expired waiting for all pending actions to complete during shutdown");
                    }

                    Monitor.Wait(_lock, timeout);
                }
            }

            _thread.Join(timeout);

            return true;
        }

        public void Kill()
        {
            lock (_lock)
            {
                _stopped = true;
                _killed = true;

                _executor.Stop();
                Monitor.PulseAll(_lock);

                // TODO trigger thread abort if necessary
            }
        }

        public override string ToString()
        {
            return string.Format("{0} (Count: {1}, Id: {2})", typeof(ThreadFiber).Name, _operations.Count,
                _thread.ManagedThreadId);
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

            IList<Execution> operations = RemoveAll();
            if (operations == null)
                return false;

            _executor.Execute(operations, remaining =>
                {
                    lock (_lock)
                    {
                        int i = 0;
                        foreach (Execution action in remaining)
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
                while (_operations.Count == 0 && !_killed && !_stopped)
                    Monitor.Wait(_lock);

                if (_killed)
                    return false;

                if (_stopped)
                    return _operations.Count > 0;
            }

            return true;
        }

        IList<Execution> RemoveAll()
        {
            lock (_lock)
            {
                IList<Execution> operations = _operations;

                _operations = new List<Execution>();

                return operations;
            }
        }
    }
}