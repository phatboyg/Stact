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
namespace Magnum.RulesEngine
{
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Text;
	using ExecutionModel;

	public class StringNodeVisitor :
		NodeVisitor
	{
		private readonly StringBuilder _output = new StringBuilder();

		public string Result
		{
			get { return _output.ToString(); }
		}

		public void Visit(Engine engine)
		{
			Visit(engine.Nodes);
		}

		protected override Node VisitRootAlpha(SingleInputTreeNode r)
		{
			_output.AppendLine("Alpha Nodes");

			r.Outputs.Each(x =>
				{
					Visit(x);
				});

			return base.VisitRootAlpha(r);
		}

		protected override Node VisitSingleInput(SingleInputNode node)
		{
			_output.AppendFormat("Single Input Node: {0}", node.InputType.Name).AppendLine();

			return base.VisitSingleInput(node);
		}

		protected override Node VisitSingleCondition(ConditionNode node)
		{
			_output.AppendFormat("\t{0} - Condition: {1}", node.InputType.Name, node.Expression).AppendLine();

			return base.VisitSingleCondition(node);
		}

	}

	public class AddConditionVisitor :
		NodeVisitor
	{
		private readonly IList<Node> _outputNodes = new List<Node>();
		private readonly Expression _expression;

		public AddConditionVisitor(Expression expression)
		{
			_expression = expression;
		}

		protected override Node VisitSingleCondition(ConditionNode node)
		{
			if (node.Expression.ToString() == _expression.ToString())
			{
				_outputNodes.Add(node);
			}

			return base.VisitSingleCondition(node);
		}
		
	}
}