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
namespace Stact.Routing.Configuration
{
	/// <summary>
	/// A configured route that can be applied and removed from a routing engine
	/// </summary>
	public interface RoutingEngineConfiguration
	{
		/// <summary>
		/// Adds the route to the engine
		/// </summary>
		/// <param name="engine"></param>
		void Apply(RoutingEngine engine);

		/// <summary>
		/// Removes the route from the engine
		/// </summary>
		/// <param name="engine"></param>
		void Remove(RoutingEngine engine);
	}
}