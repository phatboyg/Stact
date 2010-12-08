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
namespace Stact.Workflow.Internal
{
	using System.Collections;
	using System.Collections.Generic;


	public class ActivityList<TInstance> :
		IEnumerable<Activity<TInstance>>
		where TInstance : class
	{
		readonly IList<Activity<TInstance>> _events;

		public ActivityList()
		{
			_events = new List<Activity<TInstance>>();
		}

		public IEnumerator<Activity<TInstance>> GetEnumerator()
		{
			return _events.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Execute(TInstance instance)
		{
			foreach (var stateEvent in _events)
			{
				stateEvent.Execute(instance);
			}
		}

		public void Execute<TBody>(TInstance instance, TBody body)
		{
			foreach (var stateEvent in _events)
			{
				stateEvent.Execute(instance, body);
			}
		}

		public void Add(Activity<TInstance> activity)
		{
			_events.Add(activity);
		}
	}
}