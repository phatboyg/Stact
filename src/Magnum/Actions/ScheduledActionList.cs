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
	using System.Collections.Generic;
	using System.Linq;

	public class ScheduledActionList
	{
		private readonly SortedList<DateTime, List<ExecuteScheduledAction>> _actions;
		private readonly object _lock = new object();

		public ScheduledActionList()
		{
			_actions = new SortedList<DateTime, List<ExecuteScheduledAction>>();
		}

		public ExecuteScheduledAction[] GetExpiredActions(DateTime now)
		{
			lock (_lock)
			{
				ExecuteScheduledAction[] expired = _actions
					.Where(x => x.Key <= now)
					.OrderBy(x => x.Key)
					.SelectMany(x => x.Value)
					.ToArray();

				expired.Each(x =>
					{
						if (_actions.ContainsKey(x.ScheduledAt))
							_actions.Remove(x.ScheduledAt);
					});

				return expired;
			}
		}

		public bool GetNextScheduledActionTime(DateTime now, out DateTime scheduledAt)
		{
			scheduledAt = now;

			lock (_lock)
			{
				if (_actions.Count == 0)
					return false;

				foreach (KeyValuePair<DateTime, List<ExecuteScheduledAction>> pair in _actions)
				{
					if(now >= pair.Key)
						return true;

					scheduledAt = pair.Key;
					return true;
				}
			}

			return false;
		}

		public void Add(ExecuteScheduledAction action)
		{
			lock (_lock)
			{
				List<ExecuteScheduledAction> list;
				if (_actions.TryGetValue(action.ScheduledAt, out list))
				{
					list.Add(action);
				}
				else
				{
					list = new List<ExecuteScheduledAction> { action };
					_actions[action.ScheduledAt] = list;
				}
			}
		}
	}
}