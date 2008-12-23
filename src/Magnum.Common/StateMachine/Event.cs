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
namespace Magnum.Common.StateMachine
{
	using System;

	public class Event<T> : 
		Event
		where T : StateMachine<T>
	{
		private readonly string _name;

		public Event(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public override string ToString()
		{
			return _name;
		}

		public static Event<T> GetEvent(Event input)
		{
			Event<T> result = input as Event<T>;
			if (result == null)
				throw new ArgumentException("The state is not valid for this state machine", "input");

			return result;
		}
	}

	public interface Event
	{
		string Name { get; }
	}
}