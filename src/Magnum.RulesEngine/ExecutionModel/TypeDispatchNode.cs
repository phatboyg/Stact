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
	using CollectionExtensions;

	[Serializable]
	public class TypeDispatchNode :
		Node,
		Activation
	{
		private readonly Dictionary<Type, Activation> _types;

		public TypeDispatchNode()
		{
			_types = new Dictionary<Type, Activation>();
		}

		public void Activate<T>(RuleContext<T> context)
		{
			Activation activation;
			if (_types.TryGetValue(typeof (T), out activation))
			{
				activation.Activate(context);
			}
		}

		public bool Visit(NodeVisitor visitor)
		{
			return visitor.Visit(this, () => _types.Values.EachUntilFalse<Node>(x => x.Visit(visitor)));
		}

		public void Add<T>(Activation<T> successor)
		{
			Activation activation = _types.Retrieve(typeof (T), () => new TypeNode<T>());

			((TypeNode<T>) activation).AddSuccessor(successor);
		}
	}
}