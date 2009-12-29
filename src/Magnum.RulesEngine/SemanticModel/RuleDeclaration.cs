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
namespace Magnum.RulesEngine.SemanticModel
{
	using System.Collections.Generic;

	/// <summary>
	/// A rule is defined by a set of conditions and consequences. When all of the conditions are
	/// met, all of the consequences are applied.
	/// </summary>
	public class RuleDeclaration :
		Declaration
	{
		private readonly IList<ConditionDeclaration> _conditions;
		private readonly IList<ConsequenceDeclaration> _consequences;

		public RuleDeclaration(IEnumerable<ConditionDeclaration> conditions, IEnumerable<ConsequenceDeclaration> consequences)
			: base(DeclarationType.Rule)
		{
			_conditions = new List<ConditionDeclaration>(conditions);

			_consequences = new List<ConsequenceDeclaration>(consequences);
		}

		/// <summary>
		/// The conditions which must be met for the rule to be applied
		/// </summary>
		public IEnumerable<ConditionDeclaration> Conditions
		{
			get { return _conditions; }
		}

		/// <summary>
		/// The consequences to apply if the rule conditions are all satisfied
		/// </summary>
		public IEnumerable<ConsequenceDeclaration> Consequences
		{
			get { return _consequences; }
		}
	}
}