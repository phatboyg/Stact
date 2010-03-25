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
		/// Execute the actions in the list - this should only be called on the thread being 
		/// used by the action queue to avoid threading issues
		/// </summary>
		/// <returns>True if actions were executed, otherwise false</returns>
		bool Execute();

		/// <summary>
		/// Execute the actions in the list - this should only be called on the thread being 
		/// used by the action queue to avoid threading issues
		/// </summary>
		/// <param name="remaining">The number of actions remaining in the queue</param>
		/// <returns>True if actions were executed, otherwise false</returns>
		bool Execute(out int remaining);

		/// <summary>
		/// Execute all the actions in the list and only return once they have completed
		/// </summary>
		/// <param name="timeout">How long to wait (in ms) before returning anyway</param>
		/// <param name="executingActions">An extra condition to check as part of the exit clause</param>
		/// <returns>True if everything was executed</returns>
		void ExecuteAll(TimeSpan timeout, Func<bool> executingActions);

		/// <summary>
		/// Prevent new actions from being added to the list
		/// </summary>
		void StopAcceptingActions();

		/// <summary>
		/// Discard all pending actions
		/// </summary>
		void DiscardAllActions();

		/// <summary>
		/// Wake up the system?
		/// </summary>
		void Pulse();
	}
}