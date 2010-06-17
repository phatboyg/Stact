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
namespace Magnum.Specs.Concurrency
{
	using System.Linq;
	using Magnum.Concurrency;
	using NUnit.Framework;
	using TestFramework;


	[TestFixture]
	public class Working_with_a_functional_list_implementation
	{
		[Test]
		public void Should_correctly_add_an_item_to_the_list()
		{
			ImmutableList<int> list = new EmptyImmutableList<int>();

			list = list.Add(27);

			list.Count.ShouldEqual(1);
			list.First().ShouldEqual(27);
		}

		[Test]
		public void Should_correctly_add_two_items_to_the_list()
		{
			ImmutableList<int> list = new EmptyImmutableList<int>().Add(27).Add(47);

			list.Count.ShouldEqual(2);
			list.First().ShouldEqual(47);
			list.Skip(1).First().ShouldEqual(27);
		}

		[Test]
		public void Should_correctly_implement_an_empty_queue()
		{
			var empty = new EmptyImmutableList<int>();

			empty.IsEmpty.ShouldBeTrue();
		}
	}
}