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
	using System.Diagnostics;
	using System.Linq.Expressions;

	/// <summary>
	/// The interface supported by all condition (alpha) nodes in the network
	/// </summary>
	public interface ConditionNode :
		SingleInputNode
	{
		Expression Expression { get; }

		IEnumerable<Node> Successors { get; }

		void Add(Node successor);
	}

	/// <summary>
	/// A generic condition that has a single input type (alpha node)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ConditionNode<T> :
		SingleInputNode<T>,
		ConditionNode
	{
		private readonly Func<T, bool> _eval;
		private readonly Expression<Func<T, bool>> _expression;
		private readonly NodeCollection<T> _successors;

		public ConditionNode(Expression<Func<T, bool>> expression)
		{
			InputType = typeof (T);
			NodeType = NodeType.SingleConditionNode;

			_successors = new NodeCollection<T>();

			_expression = expression;
			_eval = _expression.Compile();
		}

		public Expression Expression
		{
			get { return _expression; }
		}

		public IEnumerable<Node> Successors
		{
			get
			{
				foreach (SingleInputNode<T> node in _successors)
				{
					yield return node;
				}
			}
		}

		public void Add(Node successor)
		{
			_successors.Add(successor);
		}

		public Type InputType { get; private set; }

		public void Activate(RuleContext<T> context)
		{
			if (_eval(context.Element.Object))
			{
				_successors.Each(x => x.Activate(context));

				Trace.WriteLine(_expression + " matched " + context.Element.Object);
			}
		}

		public NodeType NodeType { get; private set; }
	}
}