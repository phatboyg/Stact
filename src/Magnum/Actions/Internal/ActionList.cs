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
namespace Magnum.Actions.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Logging;

	public class ActionList :
		IActionList
	{
		private readonly List<Action> _actions = new List<Action>();
		private readonly object _lock = new object();
		private readonly ILogger _log = Logger.GetLogger<ActionList>();
		private readonly int _queueLimit;
		private readonly int _queueTimeout;
		private bool _disabled;
		private bool _waiting;

		public ActionList(int queueLimit, int queueTimeout)
		{
			_queueLimit = queueLimit;
			_queueTimeout = queueTimeout;
		}

		public int Count
		{
			get { lock (_lock) return _actions.Count; }
		}

		public void Enqueue(Action action)
		{
			lock (_lock)
			{
				if (!IsSpaceAvailable(1))
					return;

				_actions.Add(action);

				Monitor.PulseAll(_lock);
			}
		}

		public void EnqueueMany(params Action[] actions)
		{
			lock (_lock)
			{
				if (!IsSpaceAvailable(actions.Length))
					return;

				_actions.AddRange(actions);

				Monitor.PulseAll(_lock);
			}
		}

		public bool Execute()
		{
			Action[] actions = DequeueAll();
			if (actions == null)
				return false;

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

			return true;
		}

		public bool WaitAll(TimeSpan timeout, Func<bool> condition)
		{
			lock (_lock)
			{
				_waiting = true;

				Monitor.PulseAll(_lock);

				while (_actions.Count > 0 || condition())
				{
					Monitor.Wait(_lock, timeout);
				}

				_waiting = false;

				Monitor.PulseAll(_lock);

				return _actions.Count == 0 && !condition();
			}
		}

		public void Disable()
		{
			lock (_lock)
			{
				_disabled = true;
				Monitor.PulseAll(_lock);
			}
		}

		private Action[] DequeueAll()
		{
			lock (_lock)
			{
				if (ActionsAvailable())
				{
					Action[] results = _actions.ToArray();
					_actions.Clear();

					Monitor.PulseAll(_lock);

					return results.Length == 0 ? null : results;
				}
				return null;
			}
		}

		private bool ActionsAvailable()
		{
			while (_actions.Count == 0 && !_disabled && !_waiting)
			{
				Monitor.Wait(_lock);
			}

			return !_disabled;
		}

		private bool IsSpaceAvailable(int needed)
		{
			if (_disabled)
				return false;

			if (_queueLimit <= 0 || _actions.Count + needed <= _queueLimit)
				return true;

			if (_queueTimeout <= 0)
				throw new ActionQueueFullException(needed, _actions.Count, _queueLimit);

			Monitor.Wait(_lock, _queueTimeout);
			if (_disabled)
				return false;

			if (_queueLimit > 0 && _actions.Count + needed > _queueLimit)
				throw new ActionQueueFullException(needed, _actions.Count, _queueLimit);

			return true;
		}
	}
}