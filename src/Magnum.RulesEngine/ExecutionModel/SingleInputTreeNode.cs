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
	using System.Collections.Generic;
	using Collections;

	public class SingleInputTreeNode :
		Node
	{
		private readonly MultiDictionary<Type, Node> _alphaNodes;

		public SingleInputTreeNode()
		{
			_alphaNodes = new MultiDictionary<Type, Node>(false);
		}

		public IEnumerable<Node> Outputs
		{
			get { return _alphaNodes.Values; }
		}

		public NodeType NodeType
		{
			get { return NodeType.SingleInputTree; }
		}

		public void Add<T>(SingleInputNode<T> singleInputNode)
		{
			_alphaNodes.Add(typeof (T), singleInputNode);
		}

		public void Activate<T>(RuleContext<T> context)
		{
			_alphaNodes[typeof (T)].Each(x => ((SingleInputNode<T>) x).Activate(context));
		}

		public void Add(Type inputType, Node node)
		{
			_alphaNodes.Add(inputType, node);
		}
	}
}