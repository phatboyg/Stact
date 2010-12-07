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
namespace Stact.Workflow
{
	using System;
	using System.Linq.Expressions;


	/// <summary>
	/// Exposes the metadata of a state machine workflow
	/// </summary>
	public interface WorkflowDefinition
	{
		/// <summary>
		/// Retrieves the named state for the state machine workflow
		/// </summary>
		/// <param name="name">The name of the state</param>
		/// <returns>The configured State</returns>
		State GetState(string name);

		/// <summary>
		/// Retrieves the named event for the state machine workflow
		/// </summary>
		/// <param name="name">The name of the event</param>
		/// <returns>The Event</returns>
		Event GetEvent(string name);
	}


	public interface WorkflowDefinition<TWorkflow> :
		WorkflowDefinition
		where TWorkflow : class
	{
		Event GetEvent(Expression<Func<TWorkflow, Event>> eventExpression);
		Event<TBody> GetEvent<TBody>(Expression<Func<TWorkflow, Event<TBody>>> eventExpression);
		State GetState(Expression<Func<TWorkflow, State>> stateExpression);
	}
}