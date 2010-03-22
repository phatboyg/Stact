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

	public interface ActionQueue
	{
		/// <summary>
		/// Enqueue a single action to the queue
		/// </summary>
		/// <param name="action"></param>
		void Enqueue(Action action);

		/// <summary>
		/// Enqueue a series of actions to the queue
		/// </summary>
		/// <param name="actions"></param>
		void EnqueueMany(params Action[] actions);

		/// <summary>
		/// Run all remaining actions and return when complete (or when the timeout expires)
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns>True if the queue is empty when RunAll returns</returns>
		bool WaitAll(TimeSpan timeout);

		/// <summary>
		/// Disable the action queue, discarding any remaining items
		/// </summary>
		void Disable();
	}
}