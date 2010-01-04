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
	/// Implemented by nodes that have a single input type
	/// </summary>
	public interface SingleInputNode :
		Node
	{
		/// <summary>
		/// The input type accepted by the node, needed in a non-generic interface
		/// </summary>
		Type InputType { get; }

		IEnumerable<Node> Successors { get; }

		void Add(Node successor);
	}


	/// <summary>
	/// The generic specialization for nodes that have a single input type
	/// </summary>
	/// <typeparam name="T">The type of input accepted by the node</typeparam>
	public interface SingleInputNode<T> :
		SingleInputNode
	{
		void Activate(RuleContext<T> context);
	}
}