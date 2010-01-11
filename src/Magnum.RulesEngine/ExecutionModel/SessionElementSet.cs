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
namespace Magnum.RulesEngine.ExecutionModel
{
	using System;
	using System.Collections.Generic;

	public class SessionElementSet :
		IDisposable
	{
		private readonly HashSet<SessionElement> _elements;

		private bool _disposed;

		public SessionElementSet()
		{
			_elements = new HashSet<SessionElement>();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Add(SessionElement element)
		{
			_elements.Add(element);
		}

		public void Clear()
		{
			_elements.Clear();
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_elements.Clear();
			}

			_disposed = true;
		}

		~SessionElementSet()
		{
			Dispose(false);
		}
	}
}