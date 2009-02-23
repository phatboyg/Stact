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
namespace Magnum.Actors.CommandQueues
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Exceptions;

	public class ThreadPoolCommandQueue :
		CommandQueue
	{
		private readonly List<Action> _actions = new List<Action>();
		private readonly object _lock = new object();
		private bool _enabled = true;
		private bool _flushPending;

		public void Enqueue(Action action)
		{
			lock (_lock)
			{
				_actions.Add(action);
				if (!_enabled) return;
				if (_flushPending) return;

				QueueExecutor();
			}
		}

		public void EnqueueAll(params Action[] actions)
		{
			lock (_lock)
			{
				_actions.AddRange(actions);
				if (!_enabled) return;
				if (_flushPending) return;

				QueueExecutor();
			}
		}

		public void Run()
		{
		}

		public void Disable()
		{
			lock (_lock)
			{
				_enabled = false;
			}
		}

		private void ExecuteActions(object state)
		{
			Action[] actions = DequeueAll();
			if (actions == null) return;

			foreach (var action in actions)
			{
				if (_enabled == false)
					break;

				action();
			}

			lock (_lock)
			{
				if (_actions.Count > 0)
				{
					QueueExecutor();
				}
				else
				{
					_flushPending = false;
				}
			}
		}

		private void QueueExecutor()
		{
			if (!ThreadPool.QueueUserWorkItem(ExecuteActions))
				throw new QueueFullException("Unable to queue executor to thread pool");

			_flushPending = true;
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
			while (_actions.Count == 0 && _enabled)
			{
				Monitor.Wait(_lock);
			}

			return _enabled;
		}
	}
}