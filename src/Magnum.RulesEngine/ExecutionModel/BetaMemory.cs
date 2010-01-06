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

	/// <summary>
	///	A transient store of working memory elements
	/// 
	/// Derived from the beta memory concept in RETE
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BetaMemory<T> :
		Activation<T>,
		RightActivation<T>
	{
		private readonly HashSet<RuleContext<T>> _contexts;
		private readonly SuccessorSet<T> _successors;

		public BetaMemory(params Activation<T>[] successors)
		{
			_successors = new SuccessorSet<T>(successors);
			_contexts = new HashSet<RuleContext<T>>();
		}

		public BetaMemory(IEnumerable<Activation<T>> successors)
		{
			_successors = new SuccessorSet<T>(successors);
			_contexts = new HashSet<RuleContext<T>>();
		}

		public void Activate(RuleContext<T> context)
		{
			_contexts.Add(context);

			context.EnqueueAgendaAction(0, () => _successors.Activate(context));
		}

		public bool RightActivate(RuleContext<T> context)
		{
			return _contexts.Contains(context);
		}
	}
}