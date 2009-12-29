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

	public class NodeCollection :
		IEnumerable<Node>
	{
		private readonly List<Node> _nodes;

		public NodeCollection()
		{
			_nodes = new List<Node>();
		}

		IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}
	}

	public class NodeCollection<T> :
		IEnumerable<SingleInputNode<T>>
	{
		private readonly HashSet<SingleInputNode<T>> _nodes;

		public NodeCollection()
		{
			_nodes = new HashSet<SingleInputNode<T>>();
		}

		IEnumerator<SingleInputNode<T>> IEnumerable<SingleInputNode<T>>.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		public void Add(Node node)
		{
			var inputNode = node as SingleInputNode<T>;
			if(inputNode == null)
				throw new InvalidOperationException("Cannot add successor, type not supported");

			_nodes.Add(inputNode);
		}
	}
}