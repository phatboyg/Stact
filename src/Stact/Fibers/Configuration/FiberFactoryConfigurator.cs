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
namespace Stact.Configuration
{
	using System;


	/// <summary>
	///   Configures the type of fiber to be used for handling messages
	/// </summary>
	/// <typeparam name = "T"></typeparam>
	public interface FiberFactoryConfigurator<out T>
		where T : class
	{
		/// <summary>
		///   Handle on the calling thread (synchronously)
		/// </summary>
		/// <returns></returns>
		T HandleOnCallingThread();

		/// <summary>
		///   Handle on a dedicated fiber (uses the thread pool, lightweight)
		/// </summary>
		/// <returns></returns>
		T HandleOnPoolFiber();

		/// <summary>
		///   Handle on the specified fiber
		/// </summary>
		/// <param name = "fiber">The fiber to use</param>
		/// <returns></returns>
		T HandleOnFiber(Fiber fiber);

		/// <summary>
		///   Handle on a dedicated thread (operating system thread)
		/// </summary>
		/// <returns></returns>
		T HandleOnThreadFiber();

		/// <summary>
		///   Use the specified fiber factory
		/// </summary>
		/// <param name = "fiberFactory">The fiber factory to use</param>
		/// <returns></returns>
		T UseFiberFactory(FiberFactory fiberFactory);

		/// <summary>
		///   Use the specified fiber factory
		/// </summary>
		/// <param name = "fiberFactory">The fiber factory to use</param>
		/// <returns></returns>
		T UseFiberFactory(FiberFactoryEx fiberFactory);

		/// <summary>
		///   Configure the shutdown timeout for the fiber
		/// </summary>
		/// <param name = "timeout"></param>
		/// <returns></returns>
		T SetStopTimeout(TimeSpan timeout);
	}
}