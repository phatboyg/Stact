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
	using Stact.Routing.Visualizers;


	public class AlphaNodeSelector<T> :
		AbstractRoutingEngineVisitor<AlphaNodeSelector<T>>
	{
		AlphaNode<T> _alphaNode;
		TypeRouter _typeRouter;

		public void Select(RoutingEngine engine, Action<AlphaNode<T>> callback)
		{
			_typeRouter = null;
			_alphaNode = null;

			Visit(engine);

			if (_alphaNode == null)
			{
				if (_typeRouter == null)
					throw new InvalidOperationException("No router found");

				_alphaNode = _typeRouter.GetActivation(typeof(T)) as AlphaNode<T>;
				if (_alphaNode == null)
					throw new InvalidOperationException("not an alpha node");
			}

			callback(_alphaNode);
		}


		protected override bool Visit(TypeRouter channel)
		{
			_typeRouter = channel;

			return base.Visit(channel);
		}

		protected override bool Visit<TChannel>(AlphaNode<TChannel> node)
		{
			var match = node as AlphaNode<T>;
			if (match != null)
				_alphaNode = match;

			return base.Visit(node);
		}
	}
}