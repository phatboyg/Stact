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
namespace Stact.Internal
{
	using System;
	using Magnum.Extensions;


	/// <summary>
	///   A synchronous fiber will execute an action immediately on the calling thread
	///   without any protection from an exception
	/// </summary>
	public class SynchronousFiber :
		Fiber
	{
		bool _stopping;

		public void Add(Action operation)
		{
			if (_stopping)
				return;

			operation();
		}

		public void AddMany(params Action[] operations)
		{
			operations.Each(Add);
		}

		public void Stop()
		{
			_stopping = true;
		}

		public void Shutdown(TimeSpan timeout)
		{
		}
	}
}