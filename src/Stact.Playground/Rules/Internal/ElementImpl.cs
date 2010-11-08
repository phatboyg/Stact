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


	public class ElementImpl<TElement> :
		Element<TElement>
	{
		IList<ElementReference<TElement>> _alphaMemoryReferences;
		IList<NegativeJoinResult> _negativeJoinResults;
		IList<Token<TElement>> _tokens;

		public ElementImpl()
		{
			_alphaMemoryReferences = new List<ElementReference<TElement>>();
			_negativeJoinResults = new List<NegativeJoinResult>();
			_tokens = new List<Token<TElement>>();
		}

		public void AddReference(ElementReference<TElement> reference)
		{
			_alphaMemoryReferences.Insert(0, reference);
		}

		public void AddToken(Token<TElement> token)
		{
			_tokens.Insert(0, token);
		}
	}
}