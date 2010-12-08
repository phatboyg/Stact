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
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Magnum.Extensions;


	public class StateMachineState<TInstance> :
		State<TInstance>
		where TInstance : class
	{
		readonly SimpleEvent _entry;
		readonly IDictionary<Event, ActivityList<TInstance>> _eventActivities;
		readonly SimpleEvent _exit;
		readonly string _name;

		public StateMachineState(string name)
		{
			_name = name;

			Expression<Func<State, Event>> entry = x => x.Entry;
			Expression<Func<State, Event>> exit = x => x.Exit;
			
			string entryName = entry.MemberName();
			string exitName = exit.MemberName();

			_entry = new SimpleEvent(Name + "." + entryName);
			_exit = new SimpleEvent(Name + "." + exitName);

			_eventActivities = new Dictionary<Event, ActivityList<TInstance>>();
		}

		public void Accept(StateMachineVisitor visitor)
		{
			visitor.Visit(this);

			_eventActivities
				.OrderBy(x => x.Key)
				.Select(x => x.Value)
				.SelectMany(x => x)
				.Each(x => x.Accept(visitor));
		}

		public Event Entry
		{
			get { return _entry; }
		}

		public Event Exit
		{
			get { return _exit; }
		}

		public string Name
		{
			get { return _name; }
		}

		public void RaiseEvent(TInstance instance, Event eevent)
		{
			ActivityList<TInstance> activityList;
			if (_eventActivities.TryGetValue(eevent, out activityList))
				activityList.Execute(instance);
		}

		public void RaiseEvent<TBody>(TInstance instance, Event<TBody> eevent, TBody body)
		{
			ActivityList<TInstance> activityList;
			if (_eventActivities.TryGetValue(eevent, out activityList))
				activityList.Execute(instance, body);
		}

		public void AddActivity(Activity<TInstance> activity)
		{
			ActivityList<TInstance> activityList = _eventActivities.Retrieve(activity.Event, () => new ActivityList<TInstance>());

			activityList.Add(activity);
		}

		public override string ToString()
		{
			return _name;
		}

		public bool Equals(StateMachineState<TInstance> other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Equals(other._name, _name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(StateMachineState<TInstance>))
				return false;
			return Equals((StateMachineState<TInstance>)obj);
		}

		public override int GetHashCode()
		{
			return (_name != null ? _name.GetHashCode() : 0);
		}
	}
}