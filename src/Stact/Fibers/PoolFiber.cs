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
namespace Stact.Fibers
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using Magnum.Concurrency;
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

		readonly Atomic<ImmutableList<Action>> _operations = Atomic.Create(ImmutableList<Action>.EmptyList);

		bool _executorQueued;
		bool _shuttingDown;
		bool _stopping;

		protected int Count
		{
			get { return _operations.Value.Count; }
		}

		public void Add(Action operation)
		{
			Add(x => x.Add(operation));
		}

		public void AddMany(params Action[] operations)
		{
			Add(x => x.AddMany(operations));
		}

		public void Shutdown(TimeSpan timeout)
		{
			DateTime waitUntil = SystemUtil.Now + timeout;

			lock (_lock)
			{
				_shuttingDown = true;
				Monitor.PulseAll(_lock);

				while (_operations.Value.Count > 0 || _executorQueued)
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

		void Add(Func<ImmutableList<Action>, ImmutableList<Action>> mutator)
		{
			if (_shuttingDown)
				throw new FiberException("The fiber is no longer accepting actions");

			ImmutableList<Action> previous = _operations.Set(mutator);
			if (previous.Count == 0)
			{
				lock (_lock)
				{
					if (!_executorQueued)
						QueueWorkItem();
				}
			}
		}

		void QueueWorkItem()
		{
			if (!ThreadPool.QueueUserWorkItem(x => Execute()))
				throw new FiberException("QueueUserWorkItem did not accept our execute method");

			_executorQueued = true;
		}

		bool Execute()
		{
			IEnumerable<Action> operations = RemoveAll();

			ExecuteActions(operations);

			lock (_lock)
			{
				if (_operations.Value.Count > 0)
					QueueWorkItem();
				else
				{
					_executorQueued = false;

					Monitor.PulseAll(_lock);
				}
			}

			return true;
		}

		void ExecuteActions(IEnumerable<Action> operations)
		{
			foreach (Action operation in operations)
			{
				if (_stopping)
					break;

				operation();
			}
		}

		IEnumerable<Action> RemoveAll()
		{
			return _operations.Set(x => ImmutableList<Action>.EmptyList);
		}
	}
}