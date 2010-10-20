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
namespace Stact
{
	public interface ActorConvention<TActor>
		where TActor : Actor
	{
		void Initialize(TActor instance, Fiber fiber, Scheduler scheduler, Inbox inbox);

		/// <summary>
		/// Determines if a convention the same as the target convention, to avoid 
		/// duplicate conventions from being added
		/// </summary>
		/// <param name="convention">The convention to compare</param>
		/// <returns>True if it matches, otherwise false</returns>
		bool Matches(ActorConvention<TActor> convention);
	}
}