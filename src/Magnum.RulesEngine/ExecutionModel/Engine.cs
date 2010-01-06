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
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using SemanticModel;

	public interface RulesEngine
	{
		void Assert<T>(RuleContext<T> element);
	}

	public class Engine :
		RulesEngine,
		ModelVisitorSite
	{
		private MatchTypeNode _root = new MatchTypeNode();


		public void Add(RuleDeclaration rule)
		{
			var compiler = new DeclarationCompiler();

			_root = compiler.Add(_root, rule);
		}

//		{
//			SingleInputNode lastAlphaNode = null;
//			SingleInputNode lastJoinNode = null;
//
//			foreach (ConditionDeclaration condition in rule.Conditions)
//			{
//				var normalizer = new ConditionNormalizer();
//
//				Expression expression = normalizer.Normalize(condition.Expression);
//
//				var conditionNode = (ConditionNode) Activator.CreateInstance(typeof (ConditionNode<>).MakeGenericType(condition.MatchType), expression);
//
//				var alphaNode = (Node) Activator.CreateInstance(typeof (AlphaNode<>).MakeGenericType(condition.MatchType));
//
//				conditionNode.Add(alphaNode);
//
//				lastAlphaNode = lastAlphaNode ?? ( alphaNode as SingleInputNode );
//				lastJoinNode = lastJoinNode ?? lastAlphaNode;
//
//
//				lastJoinNode = this.FastInvoke<Engine, SingleInputNode>(new[] {conditionNode.InputType}, "GenerateJoinNode", new object[] {alphaNode, lastJoinNode});
//
//				lastAlphaNode = alphaNode as SingleInputNode;
//
//				_alpha.Add(condition.MatchType, conditionNode);
//			}
//
//			var inputNode = (lastJoinNode ?? lastAlphaNode);
//
//			foreach (var consequence in rule.Consequences)
//			{
//				ConsequenceDeclaration declaration = consequence;
//
//				Action<RuleContext> action = x => declaration.Activate();
//
//				SingleInputNode node = this.FastInvoke<Engine, SingleInputNode>(new[] {inputNode.InputType}, "GenerateNode", new object[] { action });
//
//				inputNode.Add(node);
//			}
//		}
//
//		private SingleInputNode GenerateJoinNode<T>(TupleSource<T> left, TupleSource<T> right)
//		{
//			return new MemoryJunction<T>();
//		}
//
//		private SingleInputNode GenerateNode<T>(Action<RuleContext> action)
//		{
//			var node = new ActionConsequenceNode<T>(x => action(x));
//
//			return node;
//		}

		public void Assert<T>(RuleContext<T> context)
		{
			_root.Activate(context);
		}

		public bool Visit(ModelVisitor visitor)
		{
			return visitor.Visit(this, () => _root.Visit(visitor));
		}
	}


	public class FindConditionVisitor :
		AbstractModelVisitor<FindConditionVisitor>
	{
		private readonly object[] _conditionNodes;
		private readonly Expression[] _expressions;

		public FindConditionVisitor(Expression[] expressions)
		{
			_expressions = expressions;
			_conditionNodes = new object[_expressions.Length];
		}

		public object[] ExistingNodes
		{
			get {
				return _conditionNodes;
			}
		}

		protected bool Visit<T>(ConditionNode<T> node)
		{
			_expressions
				.Select((expression, index) => new {Text = expression.ToString(), Index = index})
				.Where(x => x.Text == node.Expression.ToString())
				.Each(match => { _conditionNodes[match.Index] = node; });

			return _conditionNodes.Where(x => x == null).Count() > 0;
		}

		private void MatchExistingConditionNode<T>(ConditionDeclaration condition, ConditionNode<T> node)
		{
			if (condition.MatchType != typeof (T))
				return;

			if (node.Expression.ToString() != condition.Expression.ToString())
				return;

			AlphaNode<T> alphaNode = node.GetAlphaNode();
		}
	}
}