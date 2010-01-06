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
	using System.Linq;
	using System.Linq.Expressions;

	/// <summary>
	/// A generic condition that has a single input type (alpha node)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ConditionNode<T> :
		Node,
		Activation<T>,
		ModelVisitorSite
	{
		private readonly Func<RuleContext<T>, bool> _condition;
		private readonly Func<T, bool> _eval;
		private readonly Expression<Func<T, bool>> _expression;

		private readonly SuccessorSet<T> _successors;

		public ConditionNode(Expression<Func<T, bool>> expression)
		{
			_successors = new SuccessorSet<T>();
			_expression = expression;
			_eval = _expression.Compile();
		}

		public Expression Expression
		{
			get { return _expression; }
		}

		public void Activate(RuleContext<T> context)
		{
			if (_eval(context.Element.Object))
			{
				context.EnqueueAgendaAction(0, () => _successors.Activate(context));
			}
		}

		public bool Visit(ModelVisitor visitor)
		{
			return visitor.Visit(this, () => _successors.Visit(visitor));
		}

		public void AddSuccessor(params Activation<T>[] successors)
		{
			successors.Each(x => _successors.Add(x));
		}

		public AlphaNode<T> GetAlphaNode()
		{
			return _successors.Get(() => new AlphaNode<T>());
		}
	}
}