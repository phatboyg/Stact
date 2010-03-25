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
		private static readonly ILogger _log = Logger.GetLogger<ActionList>();

		private readonly List<Action> _actions = new List<Action>();
		private readonly object _lock = new object();
		private readonly int _queueLimit;
		private readonly int _queueTimeout;
		private bool _discardAllActions;
		private bool _executingAllActions;
		private bool _notAcceptingActions;

		public ActionList(int queueLimit, int queueTimeout)
		{
			_queueLimit = queueLimit;
			_queueTimeout = queueTimeout;
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

		public bool Execute(out int remaining)
		{
			bool result = Execute();
			if (!result)
			{
				remaining = 0;
				return false;
			}

			lock (_lock)
			{
				remaining = _actions.Count;
				return true;
			}
		}

		public bool Execute()
		{
			Action[] actions = DequeueAll();
			if (actions == null)
				return false;

			foreach (Action action in actions)
			{
				if (_discardAllActions)
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

		public void ExecuteAll(TimeSpan timeout, Func<bool> executingActions)
		{
			DateTime giveUpAt = SystemUtil.Now + timeout;

			lock (_lock)
			{
				_executingAllActions = true;
				Monitor.PulseAll(_lock);

				while (_actions.Count > 0 || executingActions())
				{
					timeout = giveUpAt - SystemUtil.Now;
					if (timeout < TimeSpan.Zero)
						throw new ActionQueueException("Timeout expired waiting for queue to execute all pending actions");

					Monitor.Wait(_lock, timeout);
				}
			}
		}

		public void StopAcceptingActions()
		{
			lock (_lock)
			{
				_notAcceptingActions = true;
				Monitor.PulseAll(_lock);
			}
		}

		public void DiscardAllActions()
		{
			lock (_lock)
			{
				_discardAllActions = true;
				Monitor.PulseAll(_lock);
			}
		}

		public void Pulse()
		{
			lock (_lock)
			{
				Monitor.PulseAll(_lock);
			}
		}

		private Action[] DequeueAll()
		{
			lock (_lock)
			{
				if (ActionsAreAvailable())
				{
					Action[] results = _actions.ToArray();
					_actions.Clear();

					return results;
				}

				return null;
			}
		}

		private bool ActionsAreAvailable()
		{
			while (_actions.Count == 0 && !_executingAllActions)
			{
				Monitor.Wait(_lock);
			}

			return _actions.Count > 0;
		}

		private bool IsSpaceAvailable(int needed)
		{
			if (_notAcceptingActions)
				throw new ActionQueueException("The queue is no longer accepting actions");

			if (_queueLimit <= 0 || _actions.Count + needed <= _queueLimit)
				return true;

			if (_queueTimeout <= 0)
				throw new ActionQueueFullException(needed, _actions.Count, _queueLimit);

			Monitor.Wait(_lock, _queueTimeout);
			if (_notAcceptingActions)
				return false;

			if (_queueLimit > 0 && _actions.Count + needed > _queueLimit)
				throw new ActionQueueFullException(needed, _actions.Count, _queueLimit);

			return true;
		}
	}
}