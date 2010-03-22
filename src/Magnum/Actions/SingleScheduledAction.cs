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

	public class SingleScheduledAction :
		ExecuteScheduledAction
	{
		private readonly Action _action;
		private readonly ActionQueue _queue;
		private bool _cancelled;

		public SingleScheduledAction(DateTime scheduledAt, ActionQueue queue, Action action)
		{
			ScheduledAt = scheduledAt;
			_queue = queue;
			_action = action;
		}

		public DateTime ScheduledAt { get; set; }

		public void Cancel()
		{
			_cancelled = true;
		}

		public void Execute()
		{
			if (_cancelled)
				return;

			_queue.Enqueue(_action);
		}
	}
}