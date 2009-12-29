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

	public class NodeVisitor
	{
		public virtual Node Visit(Node node)
		{
			if (node == null)
				return node;

			switch (node.NodeType)
			{
				case NodeType.SingleInputTree:
					return VisitRootAlpha((SingleInputTreeNode) node);

				case NodeType.SingleInputNode:
					return VisitSingleInput((SingleInputNode)node);

				case NodeType.SingleConditionNode:
					return VisitSingleCondition((ConditionNode) node);

				default:
					throw new ArgumentException("The node is not a known type: " + node.NodeType,
						"pipeline");
			}
		}

		protected virtual Node VisitSingleCondition(ConditionNode node)
		{
			if(node == null)
				return null;

			return node;
		}

		protected virtual Node VisitSingleInput(SingleInputNode node)
		{
			if(node == null)
				return null;

			return node;
		}

		protected virtual Node VisitRootAlpha(SingleInputTreeNode r)
		{
			if (r == null)
				return null;

			return r;
		}
	}
}