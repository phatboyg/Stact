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
	using System.Linq;
	using System.Linq.Expressions;
	using SemanticModel;

	public interface RulesEngine
	{
		void Assert<T>(RuleContext<T> element);
	}

	public class Engine : 
		RulesEngine
	{
		private SingleInputTreeNode _alpha = new SingleInputTreeNode();

		public Node Nodes
		{
			get { return _alpha; }
		}

		public void Add(RuleDeclaration rule)
		{
			SingleInputNode lastAlphaNode = null;
			SingleInputNode lastJoinNode = null;

			foreach (ConditionDeclaration condition in rule.Conditions)
			{
				var normalizer = new ConditionNormalizer();

				Expression expression = normalizer.Normalize(condition.Expression);

				var conditionNode = (ConditionNode) Activator.CreateInstance(typeof (ConditionNode<>).MakeGenericType(condition.MatchType), expression);

				var alphaNode = (Node) Activator.CreateInstance(typeof (AlphaNode<>).MakeGenericType(condition.MatchType));

				conditionNode.Add(alphaNode);

				if(lastAlphaNode == null)
					lastAlphaNode = alphaNode as SingleInputNode;
				else if (lastJoinNode != null)
				{
					var joinNode = (SingleInputNode)Activator.CreateInstance(typeof(JoinNode<>).MakeGenericType(conditionNode.InputType), alphaNode, lastJoinNode);
					lastJoinNode = joinNode;
				}
				else
				{
					var joinNode = (SingleInputNode)Activator.CreateInstance(typeof(JoinNode<>).MakeGenericType(conditionNode.InputType), alphaNode, lastAlphaNode);
					lastJoinNode = joinNode;
				}

				lastAlphaNode = alphaNode as SingleInputNode;

				_alpha.Add(condition.MatchType, conditionNode);
			}


			foreach (var consequence in rule.Consequences)
			{
				ConsequenceDeclaration declaration = consequence;

				ActionConsequenceNode node = new ActionConsequenceNode(x => declaration.Activate());

				(lastJoinNode ?? lastAlphaNode).Add(node);
			}
		}

		private void ProjectCondition(ConditionDeclaration condition)
		{
		}

		public void Assert<T>(RuleContext<T> element)
		{
			_alpha.Activate(element);
		}
	}
}