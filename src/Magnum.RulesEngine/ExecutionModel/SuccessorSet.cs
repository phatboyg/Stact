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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Extensions;

	[Serializable]
	public class SuccessorSet<T> :
		IEnumerable<Activation<T>>,
		Node
	{
		private readonly HashSet<Activation<T>> _successors;

		public SuccessorSet()
		{
			_successors = new HashSet<Activation<T>>();
		}

		public SuccessorSet(IEnumerable<Activation<T>> successors)
		{
			_successors = new HashSet<Activation<T>>(successors);
		}

		public IEnumerator<Activation<T>> GetEnumerator()
		{
			return _successors.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Visit(NodeVisitor visitor)
		{
			return _successors.WhileTrue<Node>(x => x.Visit(visitor));
		}

		public void Add(Activation<T> activation)
		{
			_successors.Add(activation);
		}

		public void Activate(RuleContext<T> ruleContext)
		{
			_successors.Each(successor => ActivateSuccessor(successor, ruleContext));
		}

		public TNode Get<TNode>(Func<TNode, bool> filter, Func<TNode> onMissing)
			where TNode : class, Activation<T>
		{
			TNode result = _successors
				.Where(x => x.GetType() == typeof (TNode))
				.Cast<TNode>()
				.Where(filter)
				.FirstOrDefault();

			if (result != default(TNode))
				return result;

			result = onMissing();

			_successors.Add(result);

			return result;
		}

		public TNode Get<TNode>(Func<TNode> onMissing)
			where TNode : class, Activation<T>
		{
			return Get(x => true, onMissing);
		}

		private static void ActivateSuccessor(Activation<T> successor, RuleContext<T> ruleContext)
		{
			successor.Activate(ruleContext);
		}
	}
}