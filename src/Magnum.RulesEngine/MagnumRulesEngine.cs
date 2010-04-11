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
namespace Magnum.RulesEngine
{
	using System;
	using System.Collections.Generic;
	using ExecutionModel;
	using Extensions;
	using SemanticModel;

	[Serializable]
	public class MagnumRulesEngine :
		RulesEngine
	{
		private TypeDispatchNode _root = new TypeDispatchNode();


		public bool Visit(NodeVisitor visitor)
		{
			return visitor.Visit(this, () => _root.Visit(visitor));
		}

		public void Assert<T>(RuleContext<T> context)
		{
			_root.Activate(context);
		}

		public void Add(params RuleDeclaration[] rules)
		{
			IEnumerable<RuleDeclaration> all = rules;
			Add(all);
		}

		public void Add(IEnumerable<RuleDeclaration> rules)
		{
			var compiler = new DeclarationCompiler();

			rules.Each(rule => _root = compiler.Add(_root, rule));
		}
	}
}