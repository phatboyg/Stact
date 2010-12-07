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
	using Internal;


	public static class WorkflowInstanceExtensions
	{
		/// <summary>
		/// Wraps an instance so that it can be passed without the TInstance dependency
		/// </summary>
		/// <param name="workflow">The workflow</param>
		/// <param name="instance">The instance to wrap</param>
		/// <returns></returns>
		public static WorkflowInstance<TWorkflow> GetInstance<TWorkflow, TInstance>(
			this StateMachineWorkflow<TWorkflow, TInstance> workflow, TInstance instance)
			where TWorkflow : class
			where TInstance : class
		{
			return new WorkflowInstance<TWorkflow, TInstance>(workflow, instance);
		}
	}
}