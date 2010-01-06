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


	public class ActionConsequenceNode<T> :
		SingleInputNode<T>
	{
		private static readonly IEnumerable<Node> _none = new Node[] {};
		private readonly Expression<Action<RuleContext<T>>> _expression;
		private readonly Action<RuleContext<T>> _eval;

		public ActionConsequenceNode(Expression<Action<RuleContext<T>>> expression)
		{
			_expression = expression;

			_eval = _expression.Compile();
		}

		public void Activate(RuleContext<T> context)
		{
			context.EnqueueAgendaAction(() => _eval(context));
		}

		public NodeType NodeType
		{
			get { return NodeType.ActionConsequence; }
		}

		public Type InputType
		{
			get { return typeof (T); }
		}

		public IEnumerable<Node> Successors
		{
			get { return _none; }
		}

		public void Add(Node successor)
		{
			throw new InvalidOperationException("Cannot add successors to a final node");
		}
	}
}