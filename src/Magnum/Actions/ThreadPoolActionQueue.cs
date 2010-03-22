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
	using System.Collections.Generic;
	using System.Threading;
	using Actors;
	using Actors.Exceptions;

	public class ThreadPoolActionQueue :
		ActionQueue
	{
		private readonly List<Action> _actions = new List<Action>();
		private readonly object _lock = new object();
		private bool _disabled;
		private bool _executorQueued;

		public void Enqueue(Action action)
		{
			lock (_lock)
			{
				if (_disabled)
					return;

				_actions.Add(action);

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

				_actions.AddRange(actions);

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
			Action[] actions = DequeueAll();
			if (actions == null)
				return;

			foreach (Action action in actions)
			{
				if (_disabled)
					break;

				action();
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
				throw new QueueFullException("Unable to queue executor to thread pool");

			_executorQueued = true;
		}

		private Action[] DequeueAll()
		{
			lock (_lock)
			{
				if (ActionsAvailable())
				{
					Action[] results = _actions.ToArray();

					_actions.Clear();

					return results;
				}
				return null;
			}
		}

		private bool ActionsAvailable()
		{
			while (_actions.Count == 0 && !_disabled)
			{
				Monitor.Wait(_lock);
			}

			return !_disabled;
		}
	}
}