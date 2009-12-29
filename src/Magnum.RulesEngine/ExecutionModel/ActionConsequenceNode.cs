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
	using System.Linq.Expressions;

	public class ActionConsequenceNode :
		Node
	{
		private readonly Expression<Action<RuleContext>> _expression;
		private Action<RuleContext> _eval;

		public ActionConsequenceNode(Expression<Action<RuleContext>> expression)
		{
			_expression = expression;

			_eval = _expression.Compile();
		}

		public NodeType NodeType
		{
			get { return NodeType.ActionConsequence; }
		}

		public void Evaluate(RuleContext context)
		{
			_eval(context);
		}
	}


	public class Thejoiner
	{


		/// <summary>
		/// The left side of the join, matched against the rule context
		/// </summary>
		public Node Left { get; private set; }

		/// <summary>
		/// The right side of the join, matched against the rule context
		/// </summary>
		public Node Right { get; private set; }


		/// <summary>
		/// The nodes that are to be evaluated for each bound left/right pair
		/// </summary>
		public IEnumerable<Node> Successors { get; private set; }

	}


}