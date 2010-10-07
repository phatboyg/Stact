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
namespace Stact.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.Extensions;


	public class ScheduledOperationList
	{
		readonly object _lock = new object();
		readonly SortedList<DateTime, List<ScheduledOperationExecuter>> _operations;

		public ScheduledOperationList()
		{
			_operations = new SortedList<DateTime, List<ScheduledOperationExecuter>>();
		}

		public int Count
		{
			get
			{
				lock (_lock)
					return _operations.Count;
			}
		}

		public ScheduledOperationExecuter[] GetExpiredActions(DateTime now)
		{
			// TODO refactor to incremental removeal 
			lock (_lock)
			{
				ScheduledOperationExecuter[] expired = _operations
					.Where(x => x.Key <= now)
					.OrderBy(x => x.Key)
					.SelectMany(x => x.Value)
					.ToArray();

				expired.Each(x =>
					{
						if (_operations.ContainsKey(x.ScheduledAt))
							_operations.Remove(x.ScheduledAt);
					});

				return expired;
			}
		}

		public bool GetNextScheduledTime(DateTime now, out DateTime scheduledAt)
		{
			scheduledAt = now;

			lock (_lock)
			{
				if (_operations.Count == 0)
					return false;

				foreach (KeyValuePair<DateTime, List<ScheduledOperationExecuter>> pair in _operations)
				{
					if (now >= pair.Key)
						return true;

					scheduledAt = pair.Key;
					return true;
				}
			}

			return false;
		}

		public void Add(ScheduledOperationExecuter operation)
		{
			lock (_lock)
			{
				List<ScheduledOperationExecuter> list;
				if (_operations.TryGetValue(operation.ScheduledAt, out list))
					list.Add(operation);
				else
				{
					list = new List<ScheduledOperationExecuter>
						{
							operation
						};
					_operations[operation.ScheduledAt] = list;
				}
			}
		}
	}
}