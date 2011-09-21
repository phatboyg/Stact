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


	/// <summary>
	/// Encapsulates a value such that only one instance is retained per thread,
	/// allowing concurrency without blocking on a shared lock for access to a 
	/// static variable.
	/// 
	/// NOTE that this is NOT a global variable
	/// </summary>
	/// <typeparam name="TContainer">The container type, to ensure proper segregation from other contained types</typeparam>
	/// <typeparam name="T">The type to contain</typeparam>
	public class ThreadSingleton<TContainer, T>
		where T : class
	{
		[ThreadStatic]
		static T _value;

		readonly Func<T> _createValue;

		public ThreadSingleton(Func<T> createValue)
		{
			_createValue = createValue;
		}

		public T Value
		{
			get { return _value ?? (_value = _createValue()); }
		}
	}
}