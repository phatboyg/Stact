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

	public class MonitorAccessLock :
		AccessLock
	{
		private readonly object _lock = new object();
		private bool _disabled;

		public int Limit
		{
			get { return 1; }
		}

		public void Shutdown()
		{
			_disabled = true;
		}

		public void Execute(Action action, TimeSpan timeout)
		{
			if (timeout == TimeSpan.MaxValue)
				throw new ArgumentOutOfRangeException("timeout", "An infinite timeout is not supported (nor recommended)");

			if (_disabled)
				throw new ObjectDisposedException("The object has been shutdown");

			if (!Monitor.TryEnter(_lock, timeout))
				throw new TimeoutException("A timeout occurred waiting for a lock to become available");
			try
			{
				action();
			}
			finally
			{
				Monitor.Exit(_lock);
			}
		}
	}
}