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
	using System.Collections.Generic;
	using Internal;
	using MbUnit.Framework;
	using Model;

	[TestFixture]
	public class When_listing_objects_from_the_working_memory
	{
		[Test]
		public void Should_return_objects_of_the_requested_type()
		{
			Person p = new Person {FirstName = "Chris", LastName = "Patterson"};

			IWorkingMemory memory = new DefaultWorkingMemory();
			memory.Add(p);

			List<Person> found = new List<Person>(memory.List<Person>());
			Assert.AreEqual(1, found.Count);
		}

		[Test]
		public void Should_not_return_objects_that_are_not_of_the_requested_type()
		{
			Address a = new Address {Street = "123 American Way", City = "Anytown", State = "OK", Country = "USA"};

			IWorkingMemory memory = new DefaultWorkingMemory();
			memory.Add(a);

			List<Person> found = new List<Person>(memory.List<Person>());
			Assert.AreEqual(0, found.Count);
		}

		[Test]
		public void Should_not_return_any_objects_if_the_working_memory_is_empty()
		{
			IWorkingMemory memory = new DefaultWorkingMemory();

			List<Person> found = new List<Person>(memory.List<Person>());
			Assert.AreEqual(0, found.Count);
		}

		[Test]
		public void Should_return_multiple_objects_of_the_requested_type_if_present()
		{
			Person p = new Person {FirstName = "Chris", LastName = "Patterson"};
			Address a = new Address {Street = "123 American Way", City = "Anytown", State = "OK", Country = "USA"};
			Member m = new Member {FirstName = "Joe", LastName = "Schmoe"};

			IWorkingMemory memory = new DefaultWorkingMemory();
			memory.Add(p);
			memory.Add(a);
			memory.Add(m);

			List<Person> found = new List<Person>(memory.List<Person>());
			Assert.AreEqual(2, found.Count);
		}

		[Test]
		public void Should_return_subclasses_of_the_requested_type()
		{
			Member m = new Member {FirstName = "Joe", LastName = "Schmoe"};

			IWorkingMemory memory = new DefaultWorkingMemory();
			memory.Add(m);

			List<Person> found = new List<Person>(memory.List<Person>());
			Assert.AreEqual(1, found.Count);
		}

		[Test]
		public void Should_return_members_of_the_requested_type()
		{
			Participant p = new Participant { Individual = new Member { FirstName = "Joe", LastName = "Schmoe" } };

			IWorkingMemory memory = new DefaultWorkingMemory();
			memory.Add(p);

			List<Person> found = new List<Person>(memory.List<Person>());
			Assert.AreEqual(1, found.Count);
		}

		[Test]
		public void Should_return_members_of_the_requested_type_or_supertype()
		{
			Participant p = new Participant { Individual = new Member { FirstName = "Joe", LastName = "Schmoe" }, OtherIndividual = new Member { FirstName = "Mark", LastName = "Walberg" } };

			IWorkingMemory memory = new DefaultWorkingMemory();
			memory.Add(p);

			List<Person> found = new List<Person>(memory.List<Person>());
			Assert.AreEqual(2, found.Count);
		}

		[Test]
		public void Should_return_all_the_members_of_an_enumeration()
		{
			Party party = new Party();
			party.Attendees.Add(new Member {FirstName = "Joe", LastName = "Schmoe"});
			party.Attendees.Add(new Member {FirstName = "Marky", LastName = "Mark"});
			party.Attendees.Add(new Member {FirstName = "Billy", LastName = "Dee"});

			IWorkingMemory memory = new DefaultWorkingMemory();
			memory.Add(party);

			List<Person> found = new List<Person>(memory.List<Person>());
			Assert.AreEqual(3, found.Count);
		}
	}
}