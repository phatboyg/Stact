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
	using System.Diagnostics;
	using System.Threading;
	using Internal;
	using Logging;

	[DebuggerDisplay("{Count} Actions, [{ThreadId}] ThreadActionQueue")]
	public class ThreadActionQueue :
		AbstractActionQueue
	{
		private readonly ILogger _log = Logger.GetLogger<ThreadActionQueue>();
		private readonly Thread _thread;
		private bool _isActive;

		public ThreadActionQueue()
			: this(-1, Timeout.Infinite)
		{
		}

		protected int ThreadId
		{
			get
			{
				if(_thread != null)
					return _thread.ManagedThreadId;

				return -1;
			}
		}

		public ThreadActionQueue(int queueLimit, int queueTimeout)
			: base(queueLimit, queueTimeout)
		{
			_thread = CreateThread();

			_thread.Start();
		}

		protected override bool Active
		{
			get { return _isActive; }
		}

		public override void ExecuteAll(TimeSpan timeout)
		{
			base.ExecuteAll(timeout);

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
			_isActive = true;

			_log.Debug(x => x.Write("{0} Started", _thread.Name));

			try
			{
				while (Execute())
				{
				}
			}
			catch (Exception ex)
			{
				_log.Error(ex);
			}

			_isActive = false;

			Pulse();

			_log.Debug(x => x.Write("{0} Exiting", _thread.Name));
		}
	}
}