// Copyright 2010 Chris Patterson
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
namespace Stact.Schedulers
{
    using System;


    public class ScheduledOperationExecuterImpl :
		ScheduledOperationExecuter
	{
		private readonly Action _operation;
		private readonly Fiber _fiber;
		private bool _cancelled;

		public ScheduledOperationExecuterImpl(DateTime scheduledAt, Fiber fiber, Action operation)
		{
			ScheduledAt = scheduledAt;
			_fiber = fiber;
			_operation = operation;
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

			_fiber.Add(_operation);
		}
	}
}