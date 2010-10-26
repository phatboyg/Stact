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
namespace Stact.Routing
{
	using System;
	using Internal;
	using Visualizers;


	public class JoinNodeLocator<T> :
		AbstractRoutingEngineVisitor<JoinNodeLocator<T>>
	{
		readonly Action<JoinNode<T>> _callback;
		AlphaNode<T> _alphaNode;
		JoinNode<T> _joinNode;
		TypeRouter _typeRouter;


		public JoinNodeLocator(Action<JoinNode<T>> callback)
		{
			_callback = callback;
		}

		public void Search(UntypedChannel node)
		{
			Visit(node);

			if (_joinNode == null)
			{
				if (_alphaNode == null)
				{
					if (_typeRouter == null)
						throw new InvalidOperationException("No router found");

					_alphaNode = _typeRouter.GetActivation(typeof(T)) as AlphaNode<T>;
					if (_alphaNode == null)
						throw new InvalidOperationException("not an alpha node");
				}

				_joinNode = new JoinNode<T>(new ConstantNode<T>());
				_alphaNode.AddActivation(_joinNode);
			}

			if (_joinNode != null)
				_callback(_joinNode);
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

		protected override bool Visit<TChannel>(JoinNode<TChannel> node)
		{
			var match = node as JoinNode<T>;
			if (match != null)
			{
				var constant = match.RightActivation as ConstantNode<T>;
				if (constant != null)
				{
					_joinNode = match;
					return false;
				}
			}

			return base.Visit(node);
		}
	}
}