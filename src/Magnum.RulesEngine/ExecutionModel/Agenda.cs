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

	/// <summary>
	/// An agenda is used to schedule actions to be performed as a result of rules and elements being evaluated.
	/// </summary>
	public interface Agenda
	{
		/// <summary>
		/// Add an action to the agenda using the default priority (0).
		/// </summary>
		/// <param name="action"></param>
		void Add(Action action);

		/// <summary>
		/// Add an action to the agenda using the specified priority.
		/// </summary>
		/// <param name="priority">The priority of the action</param>
		/// <param name="action">The action to execute</param>
		void Add(int priority, Action action);

		/// <summary>
		/// Executes all pending agenda items in priority order from lowest to highest
		/// </summary>
		void Execute();
	}
}