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


	/// <summary>
	/// Wraps a callback as an IDisposable to allow it to be called when the object is disposed.
	/// </summary>
	public class DisposeCallback :
		IDisposable
	{
		readonly Action _disposeCallback;

		public DisposeCallback(Action disposeCallback)
		{
			Magnum.Guard.AgainstNull(disposeCallback);

			_disposeCallback = disposeCallback;
		}

		public void Dispose()
		{
			_disposeCallback();
		}
	}
}