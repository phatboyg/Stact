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

	/// <summary>
	/// An ActionQueue that uses the .NET ThreadPool and QueueUserWorkItem to execute
	/// actions.
	/// </summary>
	public class ThreadPoolActionQueue :
		ActionQueue
	{
		private static readonly ILogger _log = Logger.GetLogger<ThreadPoolActionQueue>();

		private readonly IActionList _actions;
		private readonly object _lock = new object();
		private bool _executorQueued;

		/// <summary>
		/// Constructs a new ThreadPoolActionQueue using the default queue settings
		/// of unlimited actions with an infinite timeout to add new actions
		/// </summary>
		public ThreadPoolActionQueue()
		{
			_actions = new ActionList(-1, Timeout.Infinite);
		}

		/// <summary>
		/// Constructs a new ThreadPoolActionQueue using the specified settings for the
		/// queue
		/// </summary>
		/// <param name="queueLimit">The maximum number of actions that can be waiting in the queue</param>
		/// <param name="queueTimeout">The timeout to wait when adding actions when the queue if full before throwing an exception</param>
		public ThreadPoolActionQueue(int queueLimit, int queueTimeout)
		{
			_actions = new ActionList(queueLimit, queueTimeout);
		}

		public void Enqueue(Action action)
		{
			_actions.Enqueue(action);

			ActionAddedToQueue();
		}

		public void EnqueueMany(params Action[] actions)
		{
			_actions.EnqueueMany(actions);

			ActionAddedToQueue();
		}

		public void StopAcceptingActions()
		{
			_actions.StopAcceptingActions();
		}

		public void DiscardAllActions()
		{
			_actions.DiscardAllActions();
		}

		public void ExecuteAll(TimeSpan timeout)
		{
			_actions.ExecuteAll(timeout, () => _executorQueued);
		}

		private void Execute(object state)
		{
            bool result = _actions.Execute();

			lock (_lock)
			{
				if (result)
				{
					QueueWorkItem();
				}
				else
				{
					_executorQueued = false;

					_actions.Pulse();
				}
			}
		}

		private void ActionAddedToQueue()
		{
			lock (_lock)
			{
				if (_executorQueued)
					return;

				QueueWorkItem();
			}
		}

		private void QueueWorkItem()
		{
			if (!ThreadPool.QueueUserWorkItem(Execute))
				throw new ActionQueueException("QueueUserWorkItem did not accept our execute method");

			_executorQueued = true;
		}
	}
}