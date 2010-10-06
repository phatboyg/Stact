// // Copyright 2010 Chris Patterson
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
namespace Stact.Configuration
{
	using Fibers;


	/// <summary>
	/// Configures the type of fiber to be used for handling messages
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface FiberProviderConfigurator<T, TKey> :
		FiberFactoryConfigurator<T>
		where T : class
	{
		/// <summary>
		/// Share one fiber for every key value
		/// </summary>
		/// <returns></returns>
		T ShareFiberAcrossInstances();

		/// <summary>
		/// Create a new fiber for each key value
		/// </summary>
		/// <returns></returns>
		T CreateFiberPerInstance();

		/// <summary>
		/// Specifies a specific fiber provider, which provides the fiber for each instance
		/// </summary>
		/// <param name="fiberProvider"></param>
		/// <returns></returns>
		T UseFiberProvider(FiberProvider<TKey> fiberProvider);
	}
}