// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Stact.Fibers.Configuration
{
	/// <summary>
	/// Configures the type of fiber to be used for handling messages
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface FiberProviderConfigurator<T, TKey>
	{
		/// <summary>
		/// Handles the message on the calling thread (synchronously)
		/// </summary>
		/// <returns></returns>
		T HandleOnCallingThread();

		/// <summary>
		/// Handles messages to all instances using a single shared fiber
		/// </summary>
		/// <returns></returns>
		T HandleOnSingleFiber();

		/// <summary>
		/// Each instance handles messages on its own fiber
		/// </summary>
		/// <returns></returns>
		T HandleOnInstanceFiber();

		/// <summary>
		/// Handles messages to all instances using a single shared thread
		/// </summary>
		/// <returns></returns>
		T HandleOnSingleThread();

		/// <summary>
		/// Handle all requests on a single fiber, specified
		/// </summary>
		/// <param name="fiber"></param>
		/// <returns></returns>
		T HandleOnFiber(Fiber fiber);

		/// <summary>
		/// Specifies a specific fiber provider, which provides the fiber for each instance
		/// </summary>
		/// <param name="fiberProvider"></param>
		/// <returns></returns>
		T UseFiberProvider(FiberProvider<TKey> fiberProvider);
	}
}