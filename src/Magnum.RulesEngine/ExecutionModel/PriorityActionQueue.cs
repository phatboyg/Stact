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
namespace Magnum.RulesEngine.ExecutionModel
{
	using System;
	using Collections;

	public class PriorityActionQueue
	{
		private readonly OrderedDictionary<int, Deque<Action>> _queue;

		public PriorityActionQueue()
		{
			_queue = new OrderedDictionary<int, Deque<Action>>();
		}

		public void Add(int priority, Action action)
		{
			if (!_queue.ContainsKey(priority))
			{
				_queue.Add(priority, new Deque<Action>());
			}

			_queue[priority].Add(action);
		}

		public void ExecuteAll()
		{
			_queue.Each(priority =>
				{
					priority.Value.Each(action =>
						{
							action();
							
						});
				});
		}

		public void Clear()
		{
			_queue.Clear();
		}
	}
}