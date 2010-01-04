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
	using System.Linq.Expressions;

	public interface ProductionNode :
		Node
	{
		
	}

	public class ProductionNode<T> :
		ProductionNode
	{
		private readonly Expression<Action> _expression;
		private Action _eval;

		public ProductionNode(Expression<Action> expression)
		{
			_expression = expression;
			_eval = _expression.Compile();
		}

		public NodeType NodeType
		{
			get { return NodeType.Production; }
		}
	}
}