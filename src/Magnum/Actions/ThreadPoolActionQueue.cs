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
	using System.Diagnostics;
	using System.Threading;
	using Internal;
	using Logging;

	/// <summary>
	/// An ActionQueue that uses the .NET ThreadPool and QueueUserWorkItem to execute
	/// actions.
	/// </summary>
	[DebuggerDisplay("{GetType().Name} ( Count: {Count} )")]
	public class ThreadPoolActionQueue :
		AbstractActionQueue
	{
		private static readonly ILogger _log = Logger.GetLogger<ThreadPoolActionQueue>();

		private bool _executorQueued;

		/// <summary>
		/// Constructs a new ThreadPoolActionQueue using the default queue settings
		/// of unlimited actions with an infinite timeout to add new actions
		/// </summary>
		public ThreadPoolActionQueue()
			: this(-1, Timeout.Infinite)
		{
		}

		/// <summary>
		/// Constructs a new ThreadPoolActionQueue using the specified settings for the
		/// queue
		/// </summary>
		/// <param name="queueLimit">The maximum number of actions that can be waiting in the queue</param>
		/// <param name="queueTimeout">The timeout to wait when adding actions when the queue if full before throwing an exception</param>
		public ThreadPoolActionQueue(int queueLimit, int queueTimeout)
			: base(queueLimit, queueTimeout)
		{
		}

		protected override bool Active
		{
			get { return _executorQueued; }
		}

		protected override void AfterExecute(bool actionsRemaining)
		{
			if (actionsRemaining)
				QueueWorkItem();
			else
				_executorQueued = false;
		}

		protected override void ActionAddedToQueue()
		{
			if (_executorQueued)
				return;

			QueueWorkItem();
		}

		protected override int ActionsAreAvailable()
		{
			int count = base.ActionsAreAvailable();

			return 1;
		}

		private void Execute(object state)
		{
			Execute();
		}

		private void QueueWorkItem()
		{
			if (!ThreadPool.QueueUserWorkItem(Execute))
				throw new ActionQueueException("QueueUserWorkItem did not accept our execute method");

			_executorQueued = true;
		}
	}
}