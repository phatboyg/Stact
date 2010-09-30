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
namespace Stact.Specs.Concurrency
{
	using System.Linq;
	using Stact.Concurrency;
	using NUnit.Framework;
	using TestFramework;


	[TestFixture]
	public class Working_with_a_functional_queue_implementation
	{
		[Test]
		public void Should_add_elements_to_the_front()
		{
			int value;
			ImmutableQueue<int> queue =
				ImmutableQueue<int>.EmptyQueue.AddLast(27).AddLast(47).AddLast(57).AddFirst(69).RemoveFirst(out value);

			value.ShouldEqual(69);
			queue.Count.ShouldEqual(3);
			queue.SequenceEqual(new[] {27, 47, 57}).ShouldBeTrue();
		}

		[Test]
		public void Should_add_first_to_a_single_element_queue()
		{
			ImmutableQueue<int> queue = ImmutableQueue<int>.EmptyQueue.AddFirst(27).AddFirst(47);

			queue.Count.ShouldEqual(2);
			queue.First.ShouldEqual(47);
			queue.SequenceEqual(new[] {47, 27}).ShouldBeTrue();
		}

		[Test]
		public void Should_property_function_as_a_queue()
		{
			ImmutableQueue<int> queue = ImmutableQueue<int>.EmptyQueue.AddLast(27).AddLast(47);

			queue.SequenceEqual(new[] {27, 47}).ShouldBeTrue();
		}

		[Test]
		public void Should_remove_first_elements_properly()
		{
			int value;
			ImmutableQueue<int> queue = ImmutableQueue<int>.EmptyQueue.AddLast(27).AddLast(47).RemoveFirst(out value);

			value.ShouldEqual(27);
			queue.Count.ShouldEqual(1);
			queue.First.ShouldEqual(47);
		}

		[Test]
		public void Should_remove_first_elements_properly_from_longer_queues()
		{
			int value;
			ImmutableQueue<int> queue = ImmutableQueue<int>.EmptyQueue.AddLast(27).AddLast(47).AddLast(57).RemoveFirst(out value);

			value.ShouldEqual(27);
			queue.Count.ShouldEqual(2);
			queue.SequenceEqual(new[] {47, 57}).ShouldBeTrue();
		}

		[Test]
		public void Should_remove_from_a_single_element()
		{
			int value;
			ImmutableQueue<int> queue = ImmutableQueue<int>.EmptyQueue.AddLast(27).RemoveFirst(out value);

			value.ShouldEqual(27);
			queue.Count.ShouldEqual(0);
			queue.SequenceEqual(new int[] {}).ShouldBeTrue();
		}
	}
}