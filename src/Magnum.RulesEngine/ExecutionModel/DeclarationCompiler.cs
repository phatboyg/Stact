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
	using Extensions;
	using Reflection;
	using SemanticModel;

	public class DeclarationCompiler
	{
		private readonly ConditionNormalizer _normalizer;

		public DeclarationCompiler()
		{
			_normalizer = new ConditionNormalizer();
		}

		public TypeDispatchNode Add(TypeDispatchNode root, RuleDeclaration declaration)
		{
			ConditionDeclaration[] conditions = declaration.Conditions.ToArray();
			Expression[] expressions = conditions.Select(x => _normalizer.Normalize(x.Expression)).ToArray();

			var visitor = new FindConditionVisitor(expressions);
			root.Visit(visitor);

			object[] conditionNodes = visitor.ExistingNodes;

			for (int i = 0; i < conditions.Length; i++)
			{
				if (conditionNodes[i] == null)
				{
					conditionNodes[i] = CreateConditionNode(root, conditions[i].MatchType, expressions[i]);
				}
			}

			Node joinNode = CreateJoinNode(conditionNodes, conditions[0].MatchType);

			declaration.Consequences.Each(consequence =>
				{
					CreateActionNode(joinNode, consequence.Expression);
				});

			return root;
		}


		private Node CreateConditionNode(TypeDispatchNode root, Type matchType, Expression expression)
		{
			return this.FastInvoke<DeclarationCompiler, Node>(new[] {matchType}, "CreateConditionNodeOfT", root, expression);
		}

		private Node CreateConditionNodeOfT<T>(TypeDispatchNode root, Expression<Func<T, bool>> expression)
		{
			var conditionNode = new ConditionNode<T>(expression);

			root.Add(conditionNode);

			return conditionNode;
		}

		private Node CreateJoinNode(object[] conditionNodes, Type matchType)
		{
			var args = new object[] {conditionNodes};

			return this.FastInvoke<DeclarationCompiler, Node>(new[] {matchType}, "CreateJoinNodeOfT", args);
		}

		private Node CreateJoinNodeOfT<T>(object[] conditionNodes)
		{
			IEnumerable<AlphaNode<T>> nodes = conditionNodes
				.Cast<ConditionNode<T>>()
				.Select(x => x.GetAlphaNode());

			return GetFinalJunction(nodes);
		}

		private Node CreateActionNode(Node joinNode, Expression expression)
		{
			object[] args = new object[]{joinNode, expression};

			return this.FastInvoke<DeclarationCompiler, Node>("CreateActionNodeOfT", args);
		}

		private Node CreateActionNodeOfT<T>(JoinNode<T> joinNode, Expression<Action> expression)
		{
			Expression<Action<RuleContext<T>>> action = expression.WrapActionWithArgument<RuleContext<T>>();

			return CreateActionNodeOfT(joinNode, action);
		}

		private Node CreateActionNodeOfT<T>(JoinNode<T> joinNode, Expression<Action<RuleContext<T>>> expression)
		{
			var actionNode = new ActionNode<T>(expression);

			joinNode.AddSuccessor(actionNode);

			return actionNode;
		}

		private static JoinNode<T> GetFinalJunction<T>(IEnumerable<AlphaNode<T>> nodes)
		{
			JoinNode<T> junction;

			if (nodes.Count() == 1)
			{
				junction = nodes.Single().GetConstantJoinNode();
			}
			else if (nodes.Count() == 2)
			{
				junction = nodes.Skip(1).First().GetAlphaJoinNode(nodes.First());
			}
			else
			{
				junction = new JoinNode<T>(nodes.First());
				GetFinalJunction(nodes.Skip(1)).AddSuccessor(junction);
			}

			return junction;
		}
	}
}