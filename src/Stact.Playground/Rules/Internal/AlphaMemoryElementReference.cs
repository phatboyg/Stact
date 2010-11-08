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
	public class AlphaMemoryElementReference<T>
		: ElementReference<T>
	{
		readonly Element<T> _element;
		AlphaMemory _alphaMemory;

		public AlphaMemoryElementReference(AlphaMemory<T> alphaMemory, Element<T> element)
		{
			_alphaMemory = alphaMemory;
			_element = element;
		}

		public Element<T> Element
		{
			get { return _element; }
		}
	}
}