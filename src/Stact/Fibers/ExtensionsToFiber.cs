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
namespace Stact.Fibers
{
	using System;


	public static class ExtensionsToFiber
	{
		/// <summary>
		/// Creates a disposable object that calls Shutdown on the fiber when it is
		/// disposed
		/// </summary>
		/// <param name="fiber">The fiber to shutdown</param>
		/// <param name="timeout">The timeout to wait for the shutdown to complete</param>
		/// <returns>An IDisposable object</returns>
		public static IDisposable GetShutdownDisposable(this Fiber fiber, TimeSpan timeout)
		{
			return new DisposeCallback(() => fiber.Shutdown(timeout));
		}

		/// <summary>
		/// Creates a disposable object that calls Stop on the fiber when it is
		/// disposed
		/// </summary>
		/// <param name="fiber">The fiber to stop</param>
		/// <returns>An IDisposable object</returns>
		public static IDisposable GetStopDisposable(this Fiber fiber)
		{
			return new DisposeCallback(fiber.Stop);
		}
	}
}