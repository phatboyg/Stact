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
namespace Magnum.Actions
{
	using System;
	using System.Threading;
	using Internal;
	using Logging;

	public class ThreadPoolActionQueue :
		ActionQueue
	{
		private readonly IActionList _actions;
		private readonly object _lock = new object();
		private readonly ILogger _log = Logger.GetLogger<ThreadPoolActionQueue>();
		private bool _disabled;
		private bool _executorQueued;

		public ThreadPoolActionQueue()
		{
			_actions = new ActionList(-1, Timeout.Infinite);
		}

		public ThreadPoolActionQueue(int queueLimit, int queueTimeout)
		{
			_actions = new ActionList(queueLimit, queueTimeout);
		}

		public void Enqueue(Action action)
		{
			lock (_lock)
			{
				if (_disabled)
					return;

				_actions.Enqueue(action);

				if (_executorQueued)
					return;

				QueueExecute();
			}
		}

		public void EnqueueMany(params Action[] actions)
		{
			lock (_lock)
			{
				if (_disabled)
					return;

				_actions.EnqueueMany(actions);

				if (_executorQueued)
					return;

				QueueExecute();
			}
		}

		public void Disable()
		{
			lock (_lock)
			{
				_disabled = true;
				_actions.Disable();

				Monitor.PulseAll(_lock);
			}
		}

		public bool RunAll(TimeSpan timeout)
		{
			lock (_lock)
			{
				while (_actions.Count > 0 || _executorQueued)
				{
					Monitor.Wait(_lock, timeout);
				}

				return _actions.Count == 0 && !_executorQueued;
			}
		}

		private void Execute(object state)
		{
			Action[] actions = _actions.DequeueAll();
			if (actions == null)
				return;

			foreach (Action action in actions)
			{
				if (_disabled)
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

			lock (_lock)
			{
				if (_actions.Count > 0)
				{
					QueueExecute();
				}
				else
				{
					_executorQueued = false;
				}
			}
		}

		private void QueueExecute()
		{
			if (!ThreadPool.QueueUserWorkItem(Execute))
				throw new ActionQueueException("QueueUserWorkItem did not accept our execute method");

			_executorQueued = true;
		}
	}
}