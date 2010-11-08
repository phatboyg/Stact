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


	class TokenImpl<T> :
		Token<T>
	{
		IEnumerable<Token<T>> _children;

		Element _element;

		IEnumerable<NegativeJoinResult> _joinResults;
		IEnumerable<NegativeJoinResult> _nccResults;
		ReteNode _node;
		Token _owner;
		Token<T> _parent;

		public TokenImpl(ReteNode node, Token<T> parent, Element<T> element)
		{
			_node = node;
			_parent = parent;
			_element = element;

			_children = Enumerable.Empty<Token<T>>();

			parent.Add(this);

			if (element != null)
				element.AddToken(this);
		}

		public void Add(Token<T> token)
		{
			_children = Enumerable.Repeat(token, 1).Concat(_children);
		}
	}
}