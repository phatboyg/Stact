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
namespace Magnum.Actors
{
	using System;

	/// <summary>
	/// A queue-backed execution context to ensure all operations are done on the actors thread
	/// </summary>
	public interface CommandQueue
	{
		/// <summary>
		/// Adds an action to the end of the command queue
		/// </summary>
		/// <param name="action"></param>
		void Enqueue(Action action);

		/// <summary>
		/// Adds a range of actions to the end of the command queue
		/// </summary>
		/// <param name="actions"></param>
		void EnqueueAll(params Action[] actions);

		/// <summary>
		/// Disables the command queue, losing all commands that are present in the queue
		/// </summary>
		void Disable();

		/// <summary>
		/// Runs the command queue on the callers thread. Does not return until the queue is
		/// disabled.
		/// </summary>
		void Run();
	}
}