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

	/// <summary>
	/// Implements a command queues that calls actions immediately on the callers thread
	/// as they are queued. This should only be used for testing and not in production.
	/// </summary>
	public class SynchronousCommandQueue :
		CommandQueue
	{
		private bool _enabled = true;

		public void EnqueueAll(params Action[] actions)
		{
			if (!_enabled) return;

			foreach (var action in actions)
			{
				if (!_enabled)
					break;

				action();
			}
		}

		public void Enqueue(Action action)
		{
			if (_enabled)
			{
				action();
			}
		}

		public void Run()
		{
			_enabled = true;
		}

		public void Disable()
		{
			_enabled = false;
		}
	}
}