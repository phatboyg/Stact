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
	using System.Linq;

	public class JoinNode<T> :
		SingleInputNodeWithSuccessors<T>
	{
		private readonly TupleSource<T> _leftNode;
		private readonly TupleSource<T> _rightNode;

		public JoinNode(TupleSource<T> leftNode, TupleSource<T> rightNode)
			: base(typeof (T), NodeType.Join)
		{
			_leftNode = leftNode;
			_rightNode = rightNode;
		}

		public override void Activate(RuleContext<T> context)
		{
			Successors.Select(x => x as SingleInputNode<T>)
				.Where(x => x != null)
				.Each(x => x.Activate(context));
		}
	}
}