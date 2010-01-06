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
	public class ConditionTreeNode<T> :
		Node,
		Activation<T>,
		ModelVisitorSite
	{
		private readonly SuccessorSet<T> _successors;

		public ConditionTreeNode()
		{
			_successors = new SuccessorSet<T>();
		}

		public void Activate(RuleContext<T> context)
		{
			_successors.Activate(context);
		}

		public bool Visit(ModelVisitor visitor)
		{
			return visitor.Visit(this, () => _successors.Visit(visitor));
		}

		public void AddSuccessor(params Activation<T>[] successors)
		{
			successors.Each(x => _successors.Add(x));
		}
	}
}