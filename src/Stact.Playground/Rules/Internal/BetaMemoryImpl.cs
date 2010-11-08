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
	using Magnum.Extensions;


	public class BetaMemoryImpl<T> :
		ReteNode,
		BetaMemory<T>
	{
		IEnumerable<ReteNode> _allChildren;
		IEnumerable<Token<T>> _tokens;

		public BetaMemoryImpl(ReteNode parent)
			: base(parent)
		{
			_allChildren = Enumerable.Empty<ReteNode>();
			_tokens = Enumerable.Empty<Token<T>>();
		}

		public void Activate(Token<T> token, Element<T> element)
		{
			Token<T> newToken = new TokenImpl<T>(this, token, element);
			_tokens = Enumerable.Repeat(newToken, 1).Concat(_tokens);

			//_children.Each(child => child.Activate(newToken));
		}
	}

	public class JoinNodeX<T> :
		ReteNode

	{
		AlphaMemory<T> _alphaMemory;
		Condition<T> _conditions;
		ReteNode _nearestAncestorWithSameAlphaMemory;

		public JoinNodeX(ReteNode parent)
			: base(parent)
		{
		}


		void Activate(Token<T> token)
		{
//			if (_parentJustSet)
//			{
//				RelinkToAlphaMemory();
//				if (!_alphaMemory.Elements.Any())
//					_parent.RemoveChild(this);
//			}

			_alphaMemory.Elements
				.Where(element => CheckConditions(token, element))
				.Each(element =>
					{
//						element.Activate(token, element);
					});
		}


		void RightActivate(Element<T> element)
		{
//			if (_alphaMemoryJustSet)
//			{
//				RelinkToBetaMemory();
//				if (!_parent.Children.Any())
//					_alphaMemory.RemoveSuccessor(this);
//			}
//
//			_betaMemory.Tokens
//				.Where(token => CheckConditions(token, element))
//				.Each(token =>
//					{
//						_children.Each(child => child.Activate(token, element));
//					});

		}

		bool CheckConditions(Token<T> token, Element<T> element)
		{
			return true;
		}
	}


	class Condition<T>
	{
	}
}