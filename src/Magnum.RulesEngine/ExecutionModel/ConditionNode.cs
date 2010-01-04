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
	using System.Diagnostics;
	using System.Linq;
	using System.Linq.Expressions;

	/// <summary>
	/// The interface supported by all condition (alpha) nodes in the network
	/// </summary>
	public interface ConditionNode :
		SingleInputNode
	{
		Expression Expression { get; }
	}

	/// <summary>
	/// A generic condition that has a single input type (alpha node)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ConditionNode<T> :
		SingleInputNodeWithSuccessors<T>,
		ConditionNode
	{
		private readonly Func<T, bool> _eval;
		private readonly Expression<Func<T, bool>> _expression;

		public ConditionNode(Expression<Func<T, bool>> expression)
			: base(typeof (T), NodeType.SingleConditionNode)
		{
			_expression = expression;
			_eval = _expression.Compile();
		}

		public Expression Expression
		{
			get { return _expression; }
		}

		public override void Activate(RuleContext<T> context)
		{
			if (_eval(context.Element.Object))
			{
				Successors.Select(x => x as SingleInputNode<T>)
					.Where(x => x != null)
					.Each(x => x.Activate(context));

				Trace.WriteLine(_expression + " matched " + context.Element.Object);
			}
		}
	}
}