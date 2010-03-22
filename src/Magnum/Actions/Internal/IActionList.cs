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
namespace Magnum.Actions.Internal
{
	using System;

	/// <summary>
	/// A list of actions
	/// </summary>
	public interface IActionList
	{
		/// <summary>
		/// Return the number of actions in the list
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Add a single action to the list
		/// </summary>
		/// <param name="action"></param>
		void Enqueue(Action action);

		/// <summary>
		/// Add a series of actions to the list
		/// </summary>
		/// <param name="actions"></param>
		void EnqueueMany(params Action[] actions);

		/// <summary>
		/// Prevent new actions from being added to the list
		/// </summary>
		void Disable();

		/// <summary>
		/// Return all actions in the list, clearing the list
		/// </summary>
		/// <returns></returns>
		Action[] DequeueAll();
	}
}