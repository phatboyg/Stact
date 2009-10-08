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
namespace Magnum.Threading
{
	using System;
	using System.Threading;

	public class SemaphoreAccessLock : 
		AccessLock
	{
		private readonly int _limit;

		private readonly Semaphore _semaphore;
		private readonly ManualResetEvent _shutdown;
		private readonly WaitHandle[] _waitHandles;

		public SemaphoreAccessLock(int limit)
		{
			_limit = limit;
			_semaphore = new Semaphore(limit, limit);
			_shutdown = new ManualResetEvent(false);

			_waitHandles = new WaitHandle[] {_shutdown, _semaphore};
		}

		public int Limit
		{
			get { return _limit; }
		}

		public void Shutdown()
		{
			_shutdown.Set();
		}

		public void Execute(Action action, TimeSpan timeout)
		{
			if (timeout == TimeSpan.MaxValue)
				throw new ArgumentOutOfRangeException("timeout", "An infinite timeout is not supported (nor recommended)");

			int result = WaitHandle.WaitAny(_waitHandles, timeout);
			if (result == WaitHandle.WaitTimeout)
				throw new TimeoutException("A timeout occurred waiting for a lock to become available");

			if (result == 0)
				throw new ObjectDisposedException("The lock has been shutdown while waiting");

			try
			{
				action();
			}
			finally
			{
				_semaphore.Release();
			}
		}
	}
}