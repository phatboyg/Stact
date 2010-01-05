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

	/// <summary>
	///	A transient store of working memory elements
	/// 
	/// Derived from the beta memory concept in RETE
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BetaMemory<T> :
		Activatable<T>
	{
		private readonly HashSet<RuleContext<T>> _contexts;
		private readonly IEnumerable<Activatable<T>> _successors;

		public BetaMemory(IEnumerable<Activatable<T>> successors)
		{
			_successors = successors;
			_contexts = new HashSet<RuleContext<T>>();
		}

		public void Activate(RuleContext<T> context)
		{
			_contexts.Add(context);
		}

		public Action<RuleContext<T>> RightActivate(RuleContext<T> context)
		{
			if (!_contexts.Contains(context))
				return null;

			return x => _successors.Each(successor => successor.Activate(x));
		}

		public void ActivateSuccessors()
		{
			_contexts.Each(context =>
				{
					_successors.Each(successor =>
						{
							successor.Activate(context);
						});
				});
		}
	}

	public class LeafNode<T>
	{
		private readonly HashSet<Activatable<T>> _successors;

		public LeafNode()
		{
			_successors = new HashSet<Activatable<T>>();
		}

		public Action<RuleContext<T>> Activate(RuleContext<T> context)
		{
			return ruleContext =>
				{
					_successors.Each(successor =>
						{
							successor.Activate(ruleContext);
						});
				};
		}

		public void AddSuccessor(Activatable<T> activatable)
		{
			_successors.Add(activatable);
		}
	}

	public class MemoryJunction<T> :
		Activatable<T>
	{
		private readonly Func<RuleContext<T>, Action<RuleContext<T>>> _condition;

		public MemoryJunction(Func<RuleContext<T>, Action<RuleContext<T>>> condition)
		{
			_condition = condition;
		}

		public void Activate(RuleContext<T> context)
		{
			Action<RuleContext<T>> action = _condition(context);
			if (action != null)
				action(context);
		}
	}
}