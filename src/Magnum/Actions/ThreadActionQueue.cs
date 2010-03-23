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

	public class ThreadActionQueue :
		ActionQueue
	{
		private readonly IActionList _actions;
		private readonly ILogger _log = Logger.GetLogger<ThreadActionQueue>();
		private readonly Thread _thread;
		private bool _isRunning;

		public ThreadActionQueue()
			: this(-1, Timeout.Infinite)
		{
		}

		public ThreadActionQueue(int queueLimit, int queueTimeout)
		{
			_actions = new ActionList(queueLimit, queueTimeout);
			_thread = CreateThread();

			_thread.Start();
		}

		public void Enqueue(Action action)
		{
			_actions.Enqueue(action);
		}

		public void EnqueueMany(params Action[] actions)
		{
			_actions.EnqueueMany(actions);
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
			_actions.ExecuteAll(timeout, () => _isRunning);

			_thread.Join(timeout);
		}

		private Thread CreateThread()
		{
			var thread = new Thread(Run);
			thread.Name = GetType().Name + "-" + thread.ManagedThreadId;
			thread.IsBackground = false;
			thread.Priority = ThreadPriority.Normal;

			return thread;
		}

		private void Run()
		{
			_isRunning = true;

			_log.Debug(x => x.Write("{0} Started", _thread.Name));
			try
			{
				while (_actions.Execute())
				{
				}
			}
			catch (Exception ex)
			{
				_log.Error(ex);
			}

			_isRunning = false;
			_actions.Pulse();

			_log.Debug(x => x.Write("{0} Exiting", _thread.Name));
		}
	}
}