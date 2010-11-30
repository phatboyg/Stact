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
namespace Stact.Internal
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Threading;
	using Magnum;


	[DebuggerDisplay("{GetType().Name} ( Count: {Count}, ThreadId: {ThreadId} )")]
	public class ThreadFiber :
		Fiber
	{
		readonly object _lock = new object();
		readonly Thread _thread;

		IList<Action> _operations = new List<Action>();

		bool _isActive;
		bool _shuttingDown;
		bool _stopping;

		public ThreadFiber()
		{
			_thread = CreateThread();
			_thread.Start();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected int ThreadId
		{
			get
			{
				if (_thread != null)
					return _thread.ManagedThreadId;

				return -1;
			}
		}

		public void Add(Action operation)
		{
			if (_shuttingDown)
				throw new FiberException("The fiber is no longer accepting actions");

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

				Monitor.PulseAll(_lock);
			}
		}

		Thread CreateThread()
		{
			var thread = new Thread(Run);
			thread.Name = GetType().Name + "-" + thread.ManagedThreadId;
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

			IList<Action> actions = RemoveAll();
			if (actions == null)
				return false;

			ExecuteActions(actions);

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

		void ExecuteActions(IList<Action> operations)
		{
			for (int i = 0; i < operations.Count; i++)
			{
				if (_stopping)
					break;

				operations[i]();
			}
		}

		IList<Action> RemoveAll()
		{
			lock (_lock)
			{
				var operations = _operations;

				_operations = new List<Action>();

				return operations;
			}
		}
	}
}