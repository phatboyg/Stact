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
	/// <summary>
	/// Right activation is used to match a node with a right node when doing a join.
	/// </summary>
	/// <typeparam name="T">The type of object being activated</typeparam>
	public interface RightActivation<T>
	{
		/// <summary>
		/// Activates the specified RuleContext to see if a match is present
		/// </summary>
		/// <param name="context">The context containing the element to match</param>
		/// <returns>True if the context should be passed down to the successors</returns>
		bool RightActivate(RuleContext<T> context);
	}
}