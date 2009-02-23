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
	using System.Threading;

	public class ThreadCommandQueue :
		AsyncCommandQueue,
		Startable
	{
		private readonly Thread _thread;

		public ThreadCommandQueue(int limit, int waitTime)
			:
				base(limit, waitTime)
		{
			_thread = new Thread(RunThread);
			_thread.Name = "ThreadCommandQueue-" + _thread.ManagedThreadId;
			_thread.IsBackground = false;
			_thread.Priority = ThreadPriority.Normal;
		}

		public Thread Thread
		{
			get { return _thread; }
		}

		public void Start()
		{
			_thread.Start();
		}

		public void Join()
		{
			_thread.Join();
		}

		private void RunThread()
		{
			try
			{
				Run();
			}
			catch (Exception)
			{
				//TODO
			}
		}
	}
}