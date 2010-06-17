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
namespace Magnum.Concurrency
{
	using System;
	using System.Threading;


	public class CountDownLatch
	{
		int _count;
		ManualResetEvent _event;

		public CountDownLatch(int count)
		{
			_count = count;
			_event = new ManualResetEvent(false);
		}

		public void CountDown()
		{
			if (Decrement())
				_event.Set();
		}

		public bool Wait(TimeSpan timeout)
		{
			return _event.WaitOne(timeout);
		}

		bool Decrement()
		{
			for (;;)
			{
				int previous = _count;
				if (previous == 0)
					return false;

				int next = previous - 1;

				int result = Interlocked.CompareExchange(ref _count, next, previous);
				if (result == previous)
					return next == 0;
			}
		}
	}
}