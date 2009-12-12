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
namespace Magnum.RulesEngine.Specs
{
	using System.Linq;
	using Commands;
	using Model;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class A_test
	{
		
	}

	public class Given_a_working_memory :
		A_test
	{
		protected WorkingMemory Memory;

		[Given]
		public void A_working_memory()
		{
			Memory = new HashSetWorkingMemory();
		}

	}

	[Scenario]
	public class When_executing_a_rule_against_working_memory :
		Given_a_working_memory
	{
		[When]
		public void Working_memory_contains_an_object()
		{
			Memory.Add(new SubmitOrder());
		}

		[Then]
		public void The_object_should_be_obtainable_from_working_memory()
		{
			Memory.List<SubmitOrder>().Count()
				.ShouldEqual(1);
		}

		[Then]
		public void An_object_should_not_be_obtainable_if_it_does_not_exist()
		{
			Memory.List<Customer>().Count()
				.ShouldEqual(0);
		}
	}
}