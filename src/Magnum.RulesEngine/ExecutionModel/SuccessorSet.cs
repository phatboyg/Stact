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
	using System.Collections;
	using System.Collections.Generic;

	public class SuccessorSet<T> :
		IEnumerable<Activatable<T>>
	{
		private readonly HashSet<Activatable<T>> _successors;

		public SuccessorSet()
		{
			_successors = new HashSet<Activatable<T>>();
		}

		public SuccessorSet(IEnumerable<Activatable<T>> successors)
		{
			_successors = new HashSet<Activatable<T>>(successors);
		}

		public IEnumerator<Activatable<T>> GetEnumerator()
		{
			return _successors.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(Activatable<T> activatable)
		{
			_successors.Add(activatable);
		}

		public void Activate(RuleContext<T> ruleContext)
		{
			_successors.Each(successor => ActivateSuccessor(successor, ruleContext));
		}

		private static void ActivateSuccessor(Activatable<T> successor, RuleContext<T> ruleContext)
		{
			successor.Activate(ruleContext);
		}
	}
}