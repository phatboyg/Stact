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
	using System.Collections.Concurrent;
	using System.Diagnostics;
	using System.Threading;
	using Magnum;


	/// <summary>
	/// An Fiber that uses the .NET ThreadPool and QueueUserWorkItem to execute
	/// actions.
	/// </summary>
	[DebuggerDisplay("{GetType().Name} ( Count: {Count} )")]
	public class PoolFiber :
		Fiber
	{
		readonly object _lock = new object();

		ConcurrentQueue<Action> _operations = new ConcurrentQueue<Action>();

		bool _executorQueued;
		bool _shuttingDown;
		bool _stopping;

		protected int Count
		{
			get { return _operations.Count; }
		}

		public void Add(Action operation)
		{
			if (_shuttingDown)
				throw new FiberException("The fiber is no longer accepting actions");

			lock (_lock)
			{
				_operations.Enqueue(operation);
				if (!_executorQueued)
					QueueWorkItem();
			}
		}

		public void AddMany(params Action[] operations)
		{
			if (_shuttingDown)
				throw new FiberException("The fiber is no longer accepting actions");

			lock (_lock)
			{
				for (int i = 0; i < operations.Length; i++)
				{
					_operations.Enqueue(operations[i]);
				}
				if (!_executorQueued)
					QueueWorkItem();
			}
		}

		public void Shutdown(TimeSpan timeout)
		{
			if (timeout == TimeSpan.Zero)
			{
				lock(_lock)
				{
					_shuttingDown = true;
				}

				return;
			}

			DateTime waitUntil = SystemUtil.Now + timeout;

			lock (_lock)
			{
				_shuttingDown = true;
				Monitor.PulseAll(_lock);

				while (!_operations.IsEmpty || _executorQueued)
				{
					timeout = waitUntil - SystemUtil.Now;
					if (timeout < TimeSpan.Zero)
						throw new FiberException("Timeout expired waiting for all pending actions to complete during shutdown");

					Monitor.Wait(_lock, timeout);
				}
			}
		}

		public void Stop()
		{
			_shuttingDown = true;
			_stopping = true;
		}

		void QueueWorkItem()
		{
			if (!ThreadPool.QueueUserWorkItem(x => Execute()))
				throw new FiberException("QueueUserWorkItem did not accept our execute method");

			_executorQueued = true;
		}

		bool Execute()
		{
			Action[] operations = RemoveAll();

			ExecuteActions(operations);

			lock (_lock)
			{
				if (_operations.IsEmpty)
				{
					_executorQueued = false;

					Monitor.PulseAll(_lock);
				}
				else
					QueueWorkItem();
			}

			return true;
		}

		void ExecuteActions(Action[] operations)
		{
			for (int i = 0; i < operations.Length; i++)
			{
				if (_stopping)
					break;

				operations[i]();
			}
		}

		Action[] RemoveAll()
		{
			lock (_lock)
			{
				var operations = _operations.ToArray();
				_operations = new ConcurrentQueue<Action>();

				return operations;
			}
		}
	}
}