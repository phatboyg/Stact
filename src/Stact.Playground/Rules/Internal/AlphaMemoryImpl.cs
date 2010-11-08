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
	using System.Collections.Generic;
	using System.Linq;


	class AlphaMemoryImpl<T> :
		AlphaMemory<T>
	{
		IEnumerable<ElementReference<T>> _elements;
		int _referenceCount;
		IEnumerable<ReteNode> _successors;


		public IEnumerable<Element<T>> Elements
		{
			get { return _elements.Select(reference => reference.Element); }
		}

		void Activate(Element<T> element)
		{
			ElementReference<T> reference = new AlphaMemoryElementReference<T>(this, element);
			element.AddReference(reference);
			_elements = Enumerable.Repeat(reference, 1).Concat(_elements);

			//_successors.Each(x => x.RightActivate(element));
		}
	}
}