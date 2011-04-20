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
namespace Stact.Routing.Internal
{
	using System;
	using Visualizers;


	public class MatchJoinNode<T> :
		AbstractRoutingEngineVisitor<MatchJoinNode<T>>
	{
		AlphaNode<T> _alphaNode;
		JoinNode<T> _join;

		public MatchJoinNode(AlphaNode<T> alphaNode, Action<JoinNode<T>> callback)
		{
			_alphaNode = alphaNode;

			Visit(alphaNode);

			Bind(callback);
		}

		void Bind(Action<JoinNode<T>> callback)
		{
			if (_join == null)
			{
				_join = new JoinNode<T>(new ConstantNode<T>());
				_alphaNode.AddActivation(_join);
			}

			callback(_join);
		}

		protected override bool Visit<TChannel>(JoinNode<TChannel> node)
		{
			var match = node as JoinNode<T>;
			if (match != null)
			{
				var constant = match.RightActivation as ConstantNode<T>;
				if (constant != null)
				{
					_join = match;
					return false;
				}
			}

			return base.Visit(node);
		}
	}
}