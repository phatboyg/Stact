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
	using System.Collections;
	using System.Collections.Generic;

	public class WorkingMemoryElementList<T> :
		IEnumerable<WorkingMemoryElement<T>>
	{
		private readonly IList<WorkingMemoryElement<T>> _elements;

		public WorkingMemoryElementList()
		{
			_elements = new List<WorkingMemoryElement<T>>();
		}

		public IEnumerator<WorkingMemoryElement<T>> GetEnumerator()
		{
			return _elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(WorkingMemoryElement<T> element)
		{
			_elements.Add(element);
		}
	}

	public class WorkingMemoryElementList
	{
		private readonly List<WorkingMemoryElement> _elements;

		public WorkingMemoryElementList()
		{
			_elements = new List<WorkingMemoryElement>();
		}

		public void Add(WorkingMemoryElement element)
		{
			_elements.Add(element);
		}
	}
}