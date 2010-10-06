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
namespace Stact.Fibers
{
	using System;


	public class SingleFiberProvider<TKey> :
		FiberProvider<TKey>
	{
		readonly Fiber _fiber;
		readonly TimeSpan _timeout;

		public SingleFiberProvider(FiberFactory factory, TimeSpan timeout)
		{
			_timeout = timeout;
			_fiber = factory();
		}

		public Fiber GetFiber(TKey key)
		{
			return _fiber;
		}

		public void Dispose()
		{
			_fiber.Shutdown(_timeout);
		}
	}
}