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
namespace Stact.Specs.Monads
{
	using System;
	using System.Linq.Expressions;
	using Stact.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Currying_expressions_should_be_fun
	{
		[Test]
		public void Make_it_so()
		{
			Expression<Func<string[], int, string>> accessor = (array, index) => array[index];

			var values = new[] {"One", "Two", "Three"};

			accessor.Compile()(values, 1).ShouldEqual("Two");

			accessor.Curry(1).Compile()(values).ShouldEqual("Two");
		}

		[Test]
		public void Replacing_a_method_argument_should_work()
		{
			string value = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			Expression<Func<string, int, string>> accessor = (s, i) => s.Substring(0, i);

			accessor.Compile()(value, 6).ShouldEqual("ABCDEF");

			accessor.Curry(6).Compile()(value).ShouldEqual("ABCDEF");
		}
	}
}