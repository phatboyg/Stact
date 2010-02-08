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
namespace Expression_Specs
{
	using System;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using Magnum.RulesEngine.ExecutionModel;
	using NUnit.Framework;

	[TestFixture]
	public class Expression_Specs
	{
		[Test]
		public void FirstTestName()
		{
			Expression<Func<string, bool>> x1 = x => x.Equals("Chris");
			Expression<Func<string, bool>> x2 = x => x == "Chris";

			var normalizer = new ConditionNormalizer();

			Expression e1 = normalizer.Normalize(x1);
			Expression e2 = normalizer.Normalize(x2);

			Trace.WriteLine(e1);
			Trace.WriteLine(e2);
		}
	}
}