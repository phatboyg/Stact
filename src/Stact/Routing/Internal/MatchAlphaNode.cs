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
	using Nodes;
	using Visualizers;


	public class MatchAlphaNode<T> :
		AbstractRoutingEngineVisitor<MatchAlphaNode<T>>
	{
		AlphaNode<T> _alpha;
		RootNode _root;

		public MatchAlphaNode(RoutingEngine engine, Action<AlphaNode<T>> callback)
		{
			Visit(engine);

			Bind(callback);
		}

		void Bind(Action<AlphaNode<T>> callback)
		{
			if (_alpha == null)
			{
				if (_root == null)
					throw new InvalidOperationException("The root node was not found.");

				_alpha = _root.GetAlphaNode<T>();
			}

			callback(_alpha);
		}

		protected override bool Visit(RootNode node)
		{
			_root = node;

			return base.Visit(node);
		}

		protected override bool Visit<TChannel>(AlphaNode<TChannel> node)
		{
			var match = node as AlphaNode<T>;
			if (match != null)
			{
				_alpha = match;
				return false;
			}

			return base.Visit(node);
		}
	}
}