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
	using System;
	using System.Linq;
	using Internal;
	using MbUnit.Framework;
	using Model;

	[TestFixture]
	public class Expression_Specs
	{
		[Test]
		public void FIRST_TEST_NAME()
		{
			DefaultWorkingMemory memory = new DefaultWorkingMemory();

			memory.Add(new Person {FirstName = "Chris", LastName = "Patterson"});
			memory.Add(new Person {FirstName = "Joe", LastName = "Schmoe"});


			Func<Person, bool> condition = (p) => p.FirstName == "Chris";

			var matches = memory.List<Person>().Where(condition).ToList();

			Assert.AreEqual(1, matches.Count);

			int result = memory.List<Person>().Where(condition).Count();

			Assert.AreEqual(1, result);
		}

		[Test]
		public void What_about_scoped_objects()
		{
			Party party = new Party();
			party.Attendees.Add(new Member { FirstName = "Chris", LastName = "Patterson" });
			party.Attendees.Add(new Member { FirstName = "Joe", LastName = "Schmoe" });

			DefaultWorkingMemory memory = new DefaultWorkingMemory();
			memory.Add(party);

			party = new Party();
			party.Attendees.Add(new Member { FirstName = "Mark", LastName = "Patterson" });
			party.Attendees.Add(new Member { FirstName = "Larry", LastName = "Schmoe" });
			memory.Add(party);

			int count = memory.List<Person>().Count();
			Assert.AreEqual(4, count);


			Func<Party, bool> partyPicker = (p) => p.Attendees.Count > 0;
			Func<Member, bool> personPicker = (p) => p.FirstName == "Mark";
			Func<Person, bool> personPicker2 = (p) => p.FirstName == "Larry";

			Func<Party, bool> attendeeSeek = (p) => p.Attendees.Where(personPicker).Count() > 0;

			int partyCount = memory.List<Party>().Where(attendeeSeek).Count();
			Assert.AreEqual(1, partyCount);
		}
	}
}