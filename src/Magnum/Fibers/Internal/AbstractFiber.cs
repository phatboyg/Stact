// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.Fibers.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Logging;

	public abstract class AbstractFiber :
		Fiber
	{
		private static readonly ILogger _log = Logger.GetLogger<AbstractFiber>();

		private readonly List<Action> _actions = new List<Action>();
		private readonly object _lock = new object();
		private readonly int _queueLimit;
		private readonly int _queueTimeout;
		private bool _stopping;
		private bool _shuttingDown;

		protected AbstractFiber(int queueLimit, int queueTimeout)
		{
			_queueLimit = queueLimit;
			_queueTimeout = queueTimeout;
		}

		protected abstract bool Active { get; }

		protected int Count
		{
			get
			{
				lock (_lock)
					return _actions.Count;
			}
		}

		public void Add(Action action)
		{
			lock (_lock)
			{
				if (!IsSpaceAvailable(1))
					return;

				_actions.Add(action);

				Monitor.PulseAll(_lock);

				ActionAddedToQueue();
			}
		}

		public void AddMany(params Action[] actions)
		{
			lock (_lock)
			{
				if (!IsSpaceAvailable(actions.Length))
					return;

				_actions.AddRange(actions);

				Monitor.PulseAll(_lock);

				ActionAddedToQueue();
			}
		}

		public virtual void Shutdown(TimeSpan timeout)
		{
			DateTime waitUntil = SystemUtil.Now + timeout;

			lock (_lock)
			{
				_shuttingDown = true;
				Monitor.PulseAll(_lock);

				while (_actions.Count > 0 || Active)
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
			lock (_lock)
			{
				_stopping = true;

				Monitor.PulseAll(_lock);
			}
		}

		protected bool Execute()
		{
			Action[] actions = DequeueAll();
			if (actions == null)
				return false;

			ExecuteActions(actions);

			lock (_lock)
			{
				AfterExecute(_actions.Count > 0);

				if(_actions.Count == 0)
					Monitor.PulseAll(_lock);
			}

			return true;
		}

		protected virtual void ActionAddedToQueue()
		{
		}

		protected virtual void AfterExecute(bool more)
		{
		}

		protected void Pulse()
		{
			lock (_lock)
			{
				Monitor.PulseAll(_lock);
			}
		}

		protected virtual int ActionsAreAvailable()
		{
			while (_actions.Count == 0 && !_shuttingDown)
			{
				Monitor.Wait(_lock);
			}

			return _actions.Count;
		}

		private void ExecuteActions(IEnumerable<Action> actions)
		{
			foreach (Action action in actions)
			{
				if (_stopping)
					break;

				try
				{
					action();
				}
				catch (Exception ex)
				{
					_log.Error(ex);
				}
			}
		}

		private Action[] DequeueAll()
		{
			lock (_lock)
			{
				if (ActionsAreAvailable() > 0)
				{
					Action[] results = _actions.ToArray();
					_actions.Clear();

					Monitor.PulseAll(_lock);

					return results;
				}

				return null;
			}
		}

		private bool IsSpaceAvailable(int needed)
		{
			if (_stopping)
				throw new FiberException("The fiber is no longer accepting actions");

			const int attempts = 100;

			int timeout = _queueTimeout/attempts;
			int attempt = 0;

			for (; attempt < attempts && _queueLimit > 0 && _actions.Count + needed > _queueLimit; attempt++)
			{
				if (_queueTimeout <= 0)
				{
					throw new FiberOverrunException(needed, _actions.Count, _queueLimit);
				}

				Monitor.Wait(_lock, timeout);
				if (_stopping)
					return false;

				if (_queueLimit <= 0 || _actions.Count + needed <= _queueLimit)
				{
					return true;
				}
			}

			if (attempt == attempts)
			{
				throw new FiberOverrunException(needed, _actions.Count, _queueLimit);
			}

			return true;
		}
	}
}