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
    using System.Linq;
    using System.Threading;


    public class ThreadFiber :
        Fiber,
        ExecutionContext
    {
        readonly object _lock = new object();
        readonly Thread _thread;
        readonly CancellationTokenSource _kill = new CancellationTokenSource();
        IList<Execution> _executions = new List<Execution>();

        bool _isActive;
        bool _killed;
        bool _stopped;

        public ThreadFiber()
        {
            _thread = CreateThread();
            _thread.Start();
        }

        public CancellationToken CancellationToken
        {
            get { return _kill.Token; }
        }

        public void Add(Execution execution)
        {
            if (_stopped)
                return;

            lock (_lock)
            {
                _executions.Add(execution);

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

                    return _executions.Count == 0 && _isActive == false;
                }
            }

            DateTime waitUntil = DateTime.Now + timeout;

            lock (_lock)
            {
                _stopped = true;

                Monitor.PulseAll(_lock);

                while (_executions.Count > 0 || _isActive)
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

                _kill.Cancel();

                Monitor.PulseAll(_lock);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} (Count: {1}, Id: {2})", typeof(ThreadFiber).Name, _executions.Count,
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

            IList<Execution> executions = RemoveAll();
            if (executions == null)
                return false;

            Execute(executions);

            lock (_lock)
            {
                if (_executions.Count == 0)
                    Monitor.PulseAll(_lock);
            }

            return true;
        }

        void Execute(IList<Execution> executions)
        {
            int index = 0;
            try
            {
                for (index = 0; index < executions.Count; index++)
                {
                    if (_killed)
                        break;

                    executions[index].Execute(this).Wait();
                }
            }
            catch (AggregateException ex)
            {
                lock (_lock)
                {
                    index++; // skip the failed operation
                    if (index < executions.Count)
                    {
                        var newExecutions = new List<Execution>(executions.Count - index + _executions.Count);
                        newExecutions.AddRange(executions.Skip(index));
                        newExecutions.AddRange(_executions);
                        _executions = newExecutions;
                    }
                }
            }
        }

        bool WaitForActions()
        {
            lock (_lock)
            {
                while (_executions.Count == 0 && !_killed && !_stopped)
                    Monitor.Wait(_lock);

                if (_killed)
                    return false;

                if (_stopped)
                    return _executions.Count > 0;
            }

            return true;
        }

        IList<Execution> RemoveAll()
        {
            lock (_lock)
            {
                IList<Execution> operations = _executions;

                _executions = new List<Execution>();

                return operations;
            }
        }
    }
}